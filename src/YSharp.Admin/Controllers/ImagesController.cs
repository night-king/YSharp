using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YSharp.Admin.Helpers;
using YSharp.Domain;
using YSharp.Models;
using YSharp.Services;
using YSharp.SDK;
using Microsoft.Extensions.Configuration;

namespace YSharp.Admin.Controllers
{
    [Authorize(Policy = "RolePolicy")]
    public class ImagesController : BaseController
    {
        readonly IImageService _imageService;

        public ImagesController(IImageService loginLogService)
        {
            _imageService = loginLogService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public IActionResult List(int page = 1, int pageSize = 20, string view = "small")
        {
            var model = _imageService.Query(page, pageSize);
            return View(model);
        }

        public IActionResult Delete(string id)
        {
            var model = _imageService.GetById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(ImageViewModel model)
        {
            var errMsg = "";
            var retsult = _imageService.Delete(model.Id, UserId, out errMsg);
            return RedirectToAction("Index", "Result", new ResultViewModel { Message = errMsg, Succeeded = retsult, Style = "Dialog", });
        }
    }
}