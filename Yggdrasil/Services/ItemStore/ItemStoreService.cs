using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Yggdrasil.DAL;
using Yggdrasil.HttpExceptions;
using Yggdrasil.Models;
using Yggdrasil.Services.PlayerRecord;

namespace Yggdrasil.Services.ItemStore
{
    public class ItemStoreService : IItemStoreService
    {
        private readonly ILogger<ItemStoreService> _logger;
        private readonly IDataAccessLayer _dataAccessLayer;
        private readonly IPlayerRecordService _playerRecordService;

        public ItemStoreService(ILogger<ItemStoreService> logger, IDataAccessLayer dataAccessLayer, IPlayerRecordService playerRecordService)
        {
            _logger = logger;
            _dataAccessLayer = dataAccessLayer;
            _playerRecordService = playerRecordService;
        }

        public async Task<List<ItemModel>> GetItemStore()
        {
            return await _dataAccessLayer.GetItemStore();
        }

        public async Task<List<InventoryItemModel>> GetPlayerInventory(string profileId)
        {
            var profile = await _playerRecordService.GetPlayerRecordByProfileId(profileId);

            if (profile == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No profile found matching profile id provided.");
            }

            return profile.Inventory;
        }

        public async Task GrantItemAdmin(string appId, GrantedItems info)
        {
            var callerProfile = await _dataAccessLayer.GetPlayerRecordByProfileId(appId);

            if (callerProfile == null || !callerProfile.IsAdmin)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Caller does not have admin status.");
            }

            await _dataAccessLayer.GrantItems(info);
        }

        public async Task InsertItemAdmin(string appId, ItemModel item)
        {
            var callerProfile = await _dataAccessLayer.GetPlayerRecordByProfileId(appId);

            if (callerProfile == null || !callerProfile.IsAdmin)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Caller does not have admin status.");
            }

            await _dataAccessLayer.InsertItem(item);
        }
    }
}
