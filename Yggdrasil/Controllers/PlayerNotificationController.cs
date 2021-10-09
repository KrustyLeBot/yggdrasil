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
    [Route("/[controller]")]
    public class PlayerNotificationController : Controller
    {
        private readonly ILogger<PlayerNotificationController> _logger;

        public PlayerNotificationController(ILogger<PlayerNotificationController> logger)
        {
            _logger = logger;
        }

        [HttpGet("toto/{id?}")]
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
