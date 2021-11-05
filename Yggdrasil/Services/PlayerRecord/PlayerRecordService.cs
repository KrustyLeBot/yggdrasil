using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Yggdrasil.DAL;
using Yggdrasil.HttpExceptions;
using Yggdrasil.Models;

namespace Yggdrasil.Services.PlayerRecord
{
    public class PlayerRecordService : IPlayerRecordService
    {
        private readonly ILogger<PlayerRecordService> _logger;
        private readonly IDataAccessLayer _dataAccessLayer;
        private readonly string _adminApiKey;

        public PlayerRecordService(ILogger<PlayerRecordService> logger, IDataAccessLayer dataAccessLayer)
        {
            _logger = logger;
            _dataAccessLayer = dataAccessLayer;

            _adminApiKey = Environment.GetEnvironmentVariable("ADMIN_API_KEY");
        }

        public async Task CreatePlayerRecord(string apiKey, PlayerRecordBaseInfo info)
        {
            if (apiKey != _adminApiKey)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Admin apiKey not valid.");
            }

            var profile = await _dataAccessLayer.GetPlayerRecordByProfileId(info.ProfileId);
            if (profile != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Profile already exist.");
            }

            await _dataAccessLayer.CreatePlayerRecord(info);
        }

        public async Task DeletePlayerRecord(string apiKey, string profileId)
        {
            if (apiKey != _adminApiKey)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Admin apiKey not valid.");
            }

            var profile = await _dataAccessLayer.GetPlayerRecordByProfileId(profileId);
            if(profile == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Profile does not exist.");
            }

            await _dataAccessLayer.DeletePlayerRecord(profileId);
        }
    }
}
