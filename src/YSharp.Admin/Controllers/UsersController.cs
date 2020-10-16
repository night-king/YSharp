using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YSharp.Admin.Helpers;
using YSharp.Domain;
using YSharp.Models;
using YSharp.SDK;
using YSharp.Services;
using Wkhtmltopdf.NetCore;

namespace YSharp.Admin.Controllers
{
    [Authorize(Policy = "RolePolicy")]
    public class UsersController : BaseController
    {
        readonly string _defaultReturnUrl = "/Users/List";
        readonly IUserService _userService;
        readonly IRoleService _roleService;

        readonly IHttpContextAccessor _access;
        readonly IGeneratePdf _generatePdf;
        readonly UserManager<ApplicationUser> _userManager;
        public UsersController(
            IUserService userService,
            IRoleService roleService,
            IHttpContextAccessor access,
            IGeneratePdf generatePdf,
             UserManager<ApplicationUser> userManager)
        {
            this._userService = userService;
            this._roleService = roleService;
            this._generatePdf = generatePdf;
            this._access = access;
            this._userManager = userManager;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult List(string key, int page = 1, int pageSize = 20)
        {
            var model = _userService.Query(key, page, pageSize);
            return View(model);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        public IActionResult New()
        {
            var roles = _roleService.Query("", 1, 1000);
            ViewData["Roles"] = SelectListHelper.Create(roles == null || roles.Results == null || roles.Results.Count() == 0 ? null : roles.Results.ToDictionary(x => x.Name, x => x.Name), "", true, "Please select");
            var model = new UserViewModel { };
            return View(model);
        }
        [HttpPost]
        public IActionResult New(UserViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                var roles = _roleService.Query("", 1, 1000);
                ViewData["Roles"] = SelectListHelper.Create(roles == null || roles.Results == null || roles.Results.Count() == 0 ? null : roles.Results.ToDictionary(x => x.Name, x => x.Name), model.Role, true, "Please select");
                return View(model);
            }
            var errMsg = "";
            var retsult = _userService.Create(model, out errMsg);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = errMsg, Succeeded = retsult, ReturnUrl = _defaultReturnUrl, Style = "Page" });
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit(string id)
        {
            var model = _userService.GetById(id);
            var roles = _roleService.Query("", 1, 1000);
            ViewData["Roles"] = SelectListHelper.Create(roles == null || roles.Results == null || roles.Results.Count() == 0 ? null : roles.Results.ToDictionary(x => x.Name, x => x.Name), model.Role, true, "Please select");
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(UserViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                var roles = _roleService.Query("", 1, 1000);
                ViewData["Roles"] = SelectListHelper.Create(roles == null || roles.Results == null || roles.Results.Count() == 0 ? null : roles.Results.ToDictionary(x => x.Name, x => x.Name), model.Role, true, "Please select");
                return View(model);
            }
            var errMsg = "";
            var retsult = _userService.Edit(model, out errMsg);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = errMsg, Succeeded = retsult, ReturnUrl = _defaultReturnUrl, Style = "Page" });
        }

        public IActionResult UnLock(string id)
        {
            var model = _userService.GetById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult UnLock(UserViewModel model)
        {
            var errMsg = "";
            var ip = _access.HttpContext.Connection.RemoteIpAddress.ToString();
            var retsult = _userService.UnLock(model.Id, out errMsg);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = errMsg, Succeeded = retsult, Style = "Dialog", });
        }

        public IActionResult Lock(string id)
        {
            var model = _userService.GetById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Lock(UserViewModel model)
        {
            var errMsg = "";
            var ip = _access.HttpContext.Connection.RemoteIpAddress.ToString();
            var retsult = _userService.Lock(model.Id, out errMsg);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = errMsg, Succeeded = retsult, Style = "Dialog", });
        }

        public IActionResult ResetPassword(string id)
        {
            var model = _userService.GetById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult ResetPassword(UserViewModel model)
        {
            var errMsg = "";
            var retsult = _userService.ResetPassword(model.Id, model.Password, out errMsg);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = errMsg, Succeeded = retsult, Style = "Dialog", });
        }

        public IActionResult Delete(string id)
        {
            var model = _userService.GetById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(UserViewModel model)
        {
            var errMsg = "";
            var retsult = _userService.Delete(model.Id, UserId, out errMsg);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = errMsg, Succeeded = retsult, Style = "Dialog", });
        }
    }
}