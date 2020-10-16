using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YSharp.Admin.Helpers;
using YSharp.Domain;
using YSharp.Models;
using YSharp.SDK;
using YSharp.Services;

namespace YSharp.Admin.Controllers
{
    [Authorize]
    public class MessagesController : BaseController
    {
        readonly IMessageService _messageService;
        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IActionResult List(string key, int page = 1, int pageSize = 20)
        {
            var model = _messageService.Query(key, page, pageSize, "", UserId);
            return View(model);
        }
    }
}