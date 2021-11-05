using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Yggdrasil.DAL;
using Yggdrasil.HttpExceptions;
using Yggdrasil.Models;

namespace Yggdrasil.Services.PlayerNotification
{
    public class PlayerNotificationService : IPlayerNotificationService
    {
        private readonly ILogger<PlayerNotificationService> _logger;
        private readonly IDataAccessLayer _dataAccessLayer;

        public PlayerNotificationService(ILogger<PlayerNotificationService> logger, IDataAccessLayer dataAccessLayer)
        {
            _logger = logger;
            _dataAccessLayer = dataAccessLayer;
        }

        public async Task SendPlayerNotification(string apiKey, PlayerNotificationModel notif)
        {
            var profile = await _dataAccessLayer.GetPlayerRecord(apiKey);

            if (profile == null)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "No profile found matching apiKey provided.");
            }

            await _dataAccessLayer.InsertNotification(notif.RecipientProfileId, profile.ProfileId, notif.Content);
        }

        public async Task<List<DBPlayerNotification>> GetAllPlayerNotifications(string apiKey)
        {
            var profile = await _dataAccessLayer.GetPlayerRecord(apiKey);

            if (profile == null)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "No profile found matching apiKey provided.");
            }

            await _dataAccessLayer.EmptyPlayerNotifications(profile.ProfileId);

            return profile.PlayerNotifications;
        }
    }
}
