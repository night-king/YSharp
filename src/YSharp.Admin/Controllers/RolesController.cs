using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YSharp.Admin.Helpers;
using YSharp.Domain;
using YSharp.Models;
using YSharp.SDK;
using YSharp.Services;

namespace YSharp.Admin.Controllers
{
    [Authorize(Policy = "RolePolicy")]
    public class RolesController : BaseController
    {
        readonly IRoleService _roleService;
        readonly IHttpContextAccessor _access;
        public RolesController(
            IRoleService roleService,
            IHttpContextAccessor access)
        {
            this._roleService = roleService;
            this._access = access;
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
            var model = _roleService.Query(key, page, pageSize);
            return View(model);
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        public IActionResult New()
        {
            var model = new RoleViewModel { };
            return View(model);
        }
        [HttpPost]
        public IActionResult New(RoleViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            var errMsg = "";
            var retsult = _roleService.Create(model, out errMsg);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = errMsg, Succeeded = retsult, Style = "Dialog", });
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit(string id)
        {
            var model = _roleService.GetById(id);
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(RoleViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            var errMsg = "";
            var retsult = _roleService.Edit(model, out errMsg);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = errMsg, Succeeded = retsult, Style = "Dialog", });
        }

        public IActionResult Delete(string id)
        {
            var model = _roleService.GetById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(RoleViewModel model)
        {
            var errMsg = "";
            var retsult = _roleService.Delete(model.Id, UserId, out errMsg);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = errMsg, Succeeded = retsult, Style = "Dialog", });
        }
        public IActionResult Permission(string id)
        {
            var permissions = _roleService.GetPermission(id);
            var helper = new MenuHelper("/", User);
            var menus = helper.GetAllMenu();
            var menuList = helper.AllToList();
            ViewData["Menus"] = menus;
            var model = new List<RolePermissionViewModel>();
            if (permissions.Count() > 0)
            {
                foreach (var menu in menuList)
                {
                    var isChecked = permissions.Any(x => x.PermissionId == menu.Id);
                    if (isChecked)
                    {
                        model.Add(new RolePermissionViewModel
                        {
                            PermissionId = menu.Id,
                            Url = menu.Action,
                            RelevantURL = menu.RelevantURL,
                        });
                    }
                }
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Permission()
        {
            var id = Request.Form["Id"];
            var permissions = new List<RolePermissionViewModel>();
            var helper = new MenuHelper("/", User);
            var menus = helper.GetAllMenu();
            var menuList = helper.AllToList();
            foreach (var menuId in Request.Form["Menu"])
            {
                var menu = menuList.FirstOrDefault(x => x.Id == menuId);
                if (menu == null) { continue; }
                permissions.Add(new RolePermissionViewModel
                {
                    PermissionId = menu.Id,
                    RelevantURL = menu.RelevantURL,
                    Url = menu.Action,
                });
            }
            _roleService.SavePermission(id, permissions);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = "", Succeeded = true, Style = "Dialog", });
        }

    }
}