using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YSharp.Models;
using Microsoft.AspNetCore.Http;

namespace YSharp.Admin.Controllers
{
    public class ResultController : Controller
    {
        public ActionResult Index(ResultViewModel model)
        {
            if (string.IsNullOrEmpty(model.Title))
            {
                model.Title = model.Succeeded ? "Operation succeeded" : "Operation failed";
            }
            if (string.IsNullOrEmpty(model.Style))
            {
                var referer = Request.Headers["Referer"].ToString().ToLower();
                if (!string.IsNullOrEmpty(referer) && referer.Contains("style=dialog"))
                {
                    model.Style = "Dialog";
                }
            }
            return View(model);
        }
    }
}