using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YSharp.Models;
using YSharp.Services;

namespace YSharp.Admin.Controllers
{
    [Authorize(Policy = "RolePolicy")]
    public class LoginLogsController : BaseController
    {
        readonly ILoginLogService _loginLogService;
        public LoginLogsController(ILoginLogService loginLogService)
        {
            _loginLogService = loginLogService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="username"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public IActionResult List(int page = 1, int pageSize = 20, string username = "", string ip = "")
        {
            var model = _loginLogService.Query(page, pageSize, username, ip);
            return View(model);
        }

    }
}