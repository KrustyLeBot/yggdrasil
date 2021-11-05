﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
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

        [HttpPost("playernotification")]
        public async Task<IActionResult> PostPlayerNotification([FromBody] PlayerNotificationModel notif)
        {
            Request.Headers.TryGetValue("Authorization", out var apiKey);

            await _playerNotificationService.SendPlayerNotification(apiKey, notif);
            return Ok();
        }

        [HttpGet("playernotification")]
        public async Task<IActionResult> GetPlayerNotification()
        {
            Request.Headers.TryGetValue("Authorization", out var apiKey);

            return Ok(await _playerNotificationService.GetAllPlayerNotifications(apiKey));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
