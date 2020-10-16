using YSharp.Domain;
using YSharp.Repository;
using YSharp.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YSharp.Services;

namespace YSharp.Admin
{
    /// <summary>
    /// 初始化数据库
    /// </summary>
    public class ApplicationDbInitializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public static void Seed(IApplicationBuilder builder)
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var uow = serviceScope.ServiceProvider.GetService<IUnitOfWork>();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var superadminUserName = "superadmin";
                var superAdmin = userManager.FindByNameAsync(superadminUserName).Result;
                if (superAdmin == null)
                {
                    superAdmin = new ApplicationUser
                    {
                        UserName = superadminUserName,
                        Id = RandomId.CreateEnhance(),
                        IsSuperAdmin = true,
                        LockoutEnabled = false,
                        Email = "superadmin@ysharp.com",
                        EmailConfirmed = true,
                        CreateDate = DateTime.Now
                    };
                    var createAdmin = userManager.CreateAsync(superAdmin, "123456").Result;
                    if (createAdmin.Succeeded)
                    {
                        Console.WriteLine("Create super admin account success!");
                    }
                }
                uow.Commit();
            }
        }
    }
}
