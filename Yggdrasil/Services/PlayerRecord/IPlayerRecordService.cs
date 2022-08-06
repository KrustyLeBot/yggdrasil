using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.Services.PlayerRecord
{
    public interface IPlayerRecordService
    {
        public Task<string> Authenticate(string email, string password);
        public Task<PlayerRecordModel> CreatePlayerRecord(PlayerRecordBaseInfo info);
        public Task DeletePlayerRecord(string callerProfileId, string profileId);
        public Task<PlayerRecordModel> GetPlayerRecordByProfileId(string profileId);
        public Task<PlayerRecordModel> GetPlayerRecordByEmail(string email);
        public Task<PlayerRecordModel> GetPlayerRecordByEmailAndPassword(string email, string password);
    }
}
