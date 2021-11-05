using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Yggdrasil.HttpExceptions;
using Yggdrasil.Models;
using Yggdrasil.Services.PlayerRecord;

namespace Yggdrasil.Controllers
{
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
        [HttpPut("admin/playerrecord")]
        public async Task<IActionResult> CreatePlayerRecord([FromBody] PlayerRecordBaseInfo info)
        {
            Request.Headers.TryGetValue("Authorization", out var apiKey);

            await _playerRecordService.CreatePlayerRecord(apiKey, info);
            return Ok();
        }

        [HttpDelete("admin/playerrecord")]
        public async Task<IActionResult> DeletePlayerRecord([FromQuery] string profileId)
        {
            Request.Headers.TryGetValue("Authorization", out var apiKey);

            await _playerRecordService.DeletePlayerRecord(apiKey, profileId);
            return Ok();
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
