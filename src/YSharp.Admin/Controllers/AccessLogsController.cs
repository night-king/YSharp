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
    public class AccessLogsController : BaseController
    {
        readonly IAccessLogService _accessLogService;
        public AccessLogsController(IAccessLogService accessLogService)
        {
            _accessLogService = accessLogService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public IActionResult List(string key, string start, string end, int page, int pageSize=20, string ip = "")
        {
            var model = _accessLogService.Query(key, start, end, page, pageSize, ip);
            return View(model);
        }

        public IActionResult Details(string id)
        {
            var model = _accessLogService.GetById(id);
            return View(model);
        }
    }
}