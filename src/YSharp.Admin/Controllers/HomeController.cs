using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YSharp.Models;
using YSharp.SDK;

namespace YSharp.Admin.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
