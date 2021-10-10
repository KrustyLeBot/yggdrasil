using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Yggdrasil.Models;
using Yggdrasil.Services.PlayerNotification;

namespace Yggdrasil.Controllers
{
    [Route("/[controller]")]
    public class PlayerNotificationController : Controller
    {
        private readonly ILogger<PlayerNotificationController> _logger;
        private readonly IPlayerNotificationService _playerNotificationService;

        public PlayerNotificationController(ILogger<PlayerNotificationController> logger, IPlayerNotificationService playerNotificationService)
        {
            _logger = logger;
            _playerNotificationService = playerNotificationService;
        }

        [HttpGet("ws")]
        public async Task Get()
        {
            Request.Headers.TryGetValue("Authorization", out var apiKey);

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var statusCode = await _playerNotificationService.CreatePlayerSocket(apiKey, HttpContext.WebSockets);

                if(statusCode != 200)
                {
                    HttpContext.Response.StatusCode = statusCode;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        [HttpPost("playernotification")]
        public async Task PostPlayerNotification([FromBody] PlayerNotificationModel notif)
        {
            Request.Headers.TryGetValue("Authorization", out var apiKey);

            var statusCode = await _playerNotificationService.SendPlayerNotification(apiKey, notif);
            HttpContext.Response.StatusCode = statusCode;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
