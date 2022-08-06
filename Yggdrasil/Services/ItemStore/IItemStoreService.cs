using System.Collections.Generic;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.Services.ItemStore
{
    public interface IItemStoreService
    {
        public Task<List<ItemModel>> GetItemStore();
        public Task<List<InventoryItemModel>> GetPlayerInventory(string profileId);
        public Task GrantItemAdmin(string appId, GrantedItems info);
        public Task InsertItemAdmin(string appId, ItemModel item);
    }
}
