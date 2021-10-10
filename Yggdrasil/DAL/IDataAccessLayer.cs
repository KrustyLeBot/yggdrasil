using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.DAL
{
    public interface IDataAccessLayer
    {
        public Task<PlayerRecordModel> GetPlayerRecord(string apiKey);
        public Task InsertOfflineNotification(string recipientProfileId, string senderProfileId, string message);
        public Task EmptyOfflineNotification(string profileId);
    }
}
