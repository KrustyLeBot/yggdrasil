using System.Collections.Generic;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.DAL
{
    public interface IDataAccessLayer
    {
        public Task<PlayerRecordModel> GetPlayerRecordByApiKey(string apiKey);
        public Task<PlayerRecordModel> GetPlayerRecordByProfileId(string profileId);
        public Task InsertNotification(string recipientProfileId, string senderProfileId, string message);
        public Task EmptyPlayerNotifications(string profileId);
        public Task CreatePlayerRecord(PlayerRecordBaseInfo info);
        public Task DeletePlayerRecord(string profileId);
    }
}
