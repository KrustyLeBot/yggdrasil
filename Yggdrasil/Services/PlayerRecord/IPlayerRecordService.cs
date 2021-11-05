using System.Collections.Generic;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.Services.PlayerRecord
{
    public interface IPlayerRecordService
    {
        public Task CreatePlayerRecord(string apiKey, PlayerRecordBaseInfo info);
        public Task DeletePlayerRecord(string apiKey, string profileId);
    }
}
