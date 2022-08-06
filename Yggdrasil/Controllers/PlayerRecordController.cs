using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Yggdrasil.Models;
using Yggdrasil.Services.PlayerRecord;

namespace Yggdrasil.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/[controller]")]
    public class PlayerRecordController : Controller
    {
        private readonly ILogger<PlayerRecordController> _logger;
        private readonly IPlayerRecordService _playerRecordService;

        public PlayerRecordController(ILogger<PlayerRecordController> logger, IPlayerRecordService playerRecordService)
        {
            _logger = logger;
            _playerRecordService = playerRecordService;
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Admin
        [HttpDelete("admin/playerrecord")]
        public async Task<IActionResult> DeletePlayerRecord([FromQuery] string profileId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                await _playerRecordService.DeletePlayerRecord(identity.FindFirst("ProfileId").Value, profileId);
                return Ok();
            }

            return Unauthorized();
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////
        // Client
        [AllowAnonymous]
        [HttpPost("client/login")]
        public async Task<IActionResult> Login([FromBody] PlayerRecordBaseInfo info)
        {
            var token = await _playerRecordService.Authenticate(info.Email, info.Password);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(new { token });
        }

        [AllowAnonymous]
        [HttpPost("client/createplayerrecord")]
        public async Task<IActionResult> CreatePlayerrecord([FromBody] PlayerRecordBaseInfo info)
        {
            PlayerRecordModel record = await _playerRecordService.CreatePlayerRecord(info);
            return Ok(record);
        }

        [HttpGet("client/getplayerrecord")]
        public async Task<IActionResult> GetPlayerRecord(string profileId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                PlayerRecordModel record = await _playerRecordService.GetPlayerRecordByProfileId(identity.FindFirst("ProfileId").Value);
                return Ok(record);
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
