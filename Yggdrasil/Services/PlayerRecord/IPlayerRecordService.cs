using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.Services.PlayerRecord
{
    public interface IPlayerRecordService
    {
        public Task<string> Authenticate(string email, string password);
        public Task<PlayerRecordRVModel> CreatePlayerRecord(PlayerRecordBaseInfo info);
        public Task DeletePlayerRecord(string callerProfileId, string profileId);

        //External use
        public Task<PlayerRecordRVModel> GetPlayerRecordByProfileId(string profileId);
        public Task<PlayerRecordRVModel> GetPlayerRecordByEmail(string email);
        public Task<PlayerRecordRVModel> GetPlayerRecordByEmailAndPassword(string email, string password);

        //Internal use
        public Task<PlayerRecordInternalModel> InternalGetPlayerRecordByProfileId(string profileId);
        public Task<PlayerRecordInternalModel> InternalGetPlayerRecordByEmail(string email);
        public Task<PlayerRecordInternalModel> InternalGetPlayerRecordByEmailAndPassword(string email, string password);
    }
}
