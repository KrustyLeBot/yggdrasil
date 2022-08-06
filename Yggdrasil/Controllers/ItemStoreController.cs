using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Yggdrasil.Models;
using Yggdrasil.Services.ItemStore;

namespace Yggdrasil.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/[controller]")]
    public class ItemStoreController : Controller
    {
        private readonly ILogger<ItemStoreController> _logger;
        private readonly IItemStoreService _itemStoreService;

        public ItemStoreController(ILogger<ItemStoreController> logger, IItemStoreService itemStoreService)
        {
            _logger = logger;
            _itemStoreService = itemStoreService;
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Admin
        [HttpPost("admin/grantitem")]
        public async Task<IActionResult> GrantItem([FromBody] GrantedItems info)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                await _itemStoreService.GrantItemAdmin(identity.FindFirst("ProfileId").Value, info);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost("admin/insertitem")]
        public async Task<IActionResult> InsertItem([FromBody] ItemModel item)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                await _itemStoreService.InsertItemAdmin(identity.FindFirst("ProfileId").Value, item);
                return Ok();
            }

            return Unauthorized();
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////
        // Client
        [HttpGet("client/getitemstore")]
        public async Task<IActionResult> GetItemStore()
        {
            return Ok(await _itemStoreService.GetItemStore());
        }

        [HttpGet("client/getplayerinventory")]
        public async Task<IActionResult> GetPlayerInventory()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                return Ok(await _itemStoreService.GetPlayerInventory(identity.FindFirst("ProfileId").Value));
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
