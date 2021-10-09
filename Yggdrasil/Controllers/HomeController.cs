using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("/Home")]
        [HttpGet("/Home/Index")]
        [HttpGet("/Home/Index/{id?}")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/Home/Privacy")]
        [HttpGet("/Home/Privacy/{id?}")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("/toto/{id?}")]
        public IActionResult TestAction(int? id)
        {
            return Ok(id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
