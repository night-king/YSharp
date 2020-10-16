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

namespace YSharp.API
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
               

                uow.Commit();
            }
        }
    }
}
