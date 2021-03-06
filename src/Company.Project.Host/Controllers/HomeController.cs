using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Company.Project.Models;
using Microsoft.AspNetCore.Authorization;
using Riven.Identity.Authorization;

namespace Company.Project.Controllers
{
    [ClaimsAuthorize]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Swagger()
        {
            return Redirect("/swagger");
        }
    }
}
