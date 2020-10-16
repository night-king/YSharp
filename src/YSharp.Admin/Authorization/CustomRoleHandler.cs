using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YSharp.Services;
using YSharp.Domain;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace YSharp.Admin.Authorization
{
    public class CustomRoleHandler : AuthorizationHandler<CustomRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRoleRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated == false)
            {
                context.Fail();
            }
            else
            {
                using (var serviceScope = ApplicationBuilder.Instance.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var uow = serviceScope.ServiceProvider.GetService<IUnitOfWork>();
                    var accessor = serviceScope.ServiceProvider.GetService<IHttpContextAccessor>();

                    var claimsIdentity = context.User.Identity as ClaimsIdentity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    if (claim == null || string.IsNullOrEmpty(claim.Value))
                    {
                        context.Fail();
                    }
                    else
                    {
                        var userId = claim.Value;
                        if (string.IsNullOrEmpty(userId))
                        {
                            context.Fail();
                        }
                        else
                        {
                            var user = uow.Set<ApplicationUser>().Find(userId);
                            if (user == null)
                            {
                                context.Fail();
                            }
                            else if (user.IsSuperAdmin)
                            {
                                context.Succeed(requirement);
                            }
                            else
                            {
                                var url = accessor.HttpContext.Request.Path.Value.ToLower();
                                var roles = uow.Set<RolePermission>().Where(x => x.Url.ToLower() == url || x.RelevantURL.ToLower().Contains(url)).Select(x => x.Role).Select(x => x.Name).ToArray();
                                if (roles.Any(x => context.User.IsInRole(x)))
                                {
                                    context.Succeed(requirement);
                                }
                                else
                                {
                                    context.Fail();
                                }
                            }
                        }
                    }

                }
            }
            return Task.CompletedTask;
        }
    }
}
