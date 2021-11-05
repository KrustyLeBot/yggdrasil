using System.Collections.Generic;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.DAL
{
    public interface IDataAccessLayer
    {
        public Task<PlayerRecordModel> GetPlayerRecord(string apiKey);
        public Task InsertNotification(string recipientProfileId, string senderProfileId, string message);
        public Task EmptyPlayerNotifications(string profileId);
    }
}
