using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace YSharp.Admin.Controllers
{
    public class BaseController : Controller
    {
        private string TryGetClaimValue(string claimType)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return null;
            }

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity.FindFirst(claimType);
            return claim == null ? null : claim.Value;
        }

        /// <summary>
        /// 用户id
        /// </summary>
        protected string UserId
        {
            get
            {
                return TryGetClaimValue(ClaimTypes.NameIdentifier);
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        protected string UserName
        {
            get
            {
                return User.Identity.Name;
            }
        }
    }
}
