using System.Collections.Generic;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.DAL
{
    public interface IDataAccessLayer
    {
        public Task<PlayerRecordModel> GetPlayerRecordByProfileId(string profileId);
        public Task<PlayerRecordModel> GetPlayerRecordByEmail(string email);
        public Task<PlayerRecordModel> GetPlayerRecordByEmailAndPassword(string email, string password);
        public Task InsertNotification(string recipientProfileId, string senderProfileId, string message);
        public Task EmptyPlayerNotifications(string profileId);
        public Task<PlayerRecordModel> CreatePlayerRecord(PlayerRecordBaseInfo info);
        public Task DeletePlayerRecord(string profileId);
        public Task<List<ItemModel>> GetItemStore();
        public Task InsertItem(ItemModel item);
        public Task GrantItems(GrantedItems info);
    }
}
