using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YSharp.Domain;
using YSharp.Repository;
using YSharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.Text.Json;
using YSharp.API.Middlewares;
using Wkhtmltopdf.NetCore;

namespace YSharp.API
{
    public static class ApplicationBuilder
    {
        public static IApplicationBuilder Instance
        {
            get; set;
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration
        {
            get;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDataProtection()
            // This helps surviving a restart: a same app will find back its keys. Just ensure to create the folder.
            .PersistKeysToFileSystem(new DirectoryInfo("\\keys\\"))
            // This helps surviving a site update: each app has its own store, building the site creates a new app
            .SetApplicationName("YSharp2")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(30));

            services.AddDbContextPool<ApplicationDbContext>(options =>
                        options.UseLazyLoadingProxies()//启用懒加载
                               .UseSqlServer(Configuration.GetConnectionString("SqlConnection")));
            services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            IMvcBuilder builder = services.AddRazorPages();
#if DEBUG

            builder.AddRazorRuntimeCompilation();
#endif
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;

                //Signin
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = false;


            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new  Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "APIs",
                    Version = "v1",
                    Description = "",
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddAntiforgery(options =>
            {
                // Set Cookie properties using CookieBuilder properties†.
                options.FormFieldName = "anti";
                options.HeaderName = "X-CSRF-TOKEN-ANTI";
                options.SuppressXFrameOptionsHeader = false;
            });
            services.Configure<PasswordHasherOptions>(options =>
                    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2
            );
          
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

         
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<ILoginLogService, LoginLogService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserService, UserService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AnyCors", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
                var cors = Configuration["cors"];
                if (!string.IsNullOrEmpty(cors))
                {
                    options.AddPolicy("CorsPolicy", builder => builder.WithOrigins(Configuration["cors"].Split(",")).AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
                }
            });
            services.AddOptions();
            services.AddMvc(options =>
            {
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            }).AddJsonOptions(options =>
            {
                //忽略循环引用
                options.JsonSerializerOptions.IgnoreNullValues =true;
                //不使用驼峰样式的key
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //忽略大小写
                options.JsonSerializerOptions.PropertyNameCaseInsensitive =false;
            });
            //https://blog.elmah.io/generate-a-pdf-from-asp-net-core-for-free/
            //Download the most recent version of wkhtmltopdf for Windows from https://wkhtmltopdf.org and place it in the directory wkhtmltopdf\Windows in your project folder. 
            //The Wkhtmltopdf.NetCore package expects a Windows folder inside the folder you specified in the setup. 
            //To make sure that the file is copied to the out directory, right-click it in Visual Studio and select Copy if newer
            services.AddWkhtmltopdf("wkhtmltopdf");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ApplicationBuilder.Instance = app;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),  specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIs");
                    c.RoutePrefix = "help";
                });
                app.UseCors("AnyCors");//仅允许开发环境跨域访问
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseCors("CorsPolicy");//仅允许开发环境跨域访问
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                //为了在nginx上获取用户端真实IP添加此代码
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            //Add our new middleware to the pipeline
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            // ===== 初始化数据 ======
          //  ApplicationDbInitializer.Seed(app);
        }
    }
}
