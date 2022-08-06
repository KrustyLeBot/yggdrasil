using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Yggdrasil.Models;
using Yggdrasil.Services.PlayerNotification;

namespace Yggdrasil.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        /////////////////////////////////////////////////////////////////////////////////////////
        // Admin
        [HttpPost("admin/playernotification")]
        public async Task<IActionResult> PostPlayerNotificationAdmin([FromBody] PlayerNotificationModel notif)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                await _playerNotificationService.SendPlayerNotificationAdmin(notif, identity.FindFirst("ProfileId").Value);
                return Ok();
            }

            return Unauthorized();
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////
        // Client
        [HttpPost("client/playernotification")]
        public async Task<IActionResult> PostPlayerNotification([FromBody] PlayerNotificationModel notif)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                await _playerNotificationService.SendPlayerNotification(identity.FindFirst("ProfileId").Value, notif);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet("client/playernotification")]
        public async Task<IActionResult> GetPlayerNotification()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                return Ok(await _playerNotificationService.GetAllPlayerNotifications(identity.FindFirst("ProfileId").Value));
            }

            return Unauthorized();
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
