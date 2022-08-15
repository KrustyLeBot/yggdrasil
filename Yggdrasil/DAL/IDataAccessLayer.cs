using System.Collections.Generic;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.DAL
{
    public interface IDataAccessLayer
    {
        public Task<PlayerRecordInternalModel> GetPlayerRecordByProfileId(string profileId);
        public Task<PlayerRecordInternalModel> GetPlayerRecordByEmail(string email);
        public Task<PlayerRecordInternalModel> GetPlayerRecordByEmailAndPassword(string email, string password);
        public Task InsertNotification(string recipientProfileId, string senderProfileId, string message);
        public Task EmptyPlayerNotifications(string profileId);
        public Task<PlayerRecordInternalModel> CreatePlayerRecord(PlayerRecordBaseInfo info);
        public Task DeletePlayerRecord(string profileId);
        public Task<List<ItemModel>> GetItemStore();
        public Task InsertItem(ItemModel item);
        public Task GrantItems(GrantedItems info);
    }
}
