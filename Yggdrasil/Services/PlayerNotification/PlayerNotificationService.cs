using Microsoft.Extensions.Logging;
using System;
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
        private readonly string _adminApiKey;

        public PlayerNotificationService(ILogger<PlayerNotificationService> logger, IDataAccessLayer dataAccessLayer)
        {
            _logger = logger;
            _dataAccessLayer = dataAccessLayer;

            _adminApiKey = Environment.GetEnvironmentVariable("ADMIN_API_KEY");
        }

        public async Task SendPlayerNotification(string apiKey, PlayerNotificationModel notif)
        {
            var profileSender = await _dataAccessLayer.GetPlayerRecordByApiKey(apiKey);
            var profileReceiver = await _dataAccessLayer.GetPlayerRecordByProfileId(notif.RecipientProfileId);

            if (profileSender == null)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "No profile found matching apiKey provided.");
            }

            if (profileReceiver == null)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "No profile found for recipent profile id.");
            }

            if(profileSender.LastPnotSentTime.AddSeconds(30) > DateTime.UtcNow)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "A pnot was already sent during the last 30 second.");
            }

            await _dataAccessLayer.InsertNotification(notif.RecipientProfileId, profileSender.ProfileId, notif.Content);
        }

        public async Task SendPlayerNotificationAdmin(string apiKey, PlayerNotificationModel notif, string senderAppId)
        {
            var profileReceiver = await _dataAccessLayer.GetPlayerRecordByProfileId(notif.RecipientProfileId);

            if (apiKey != _adminApiKey)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Admin apiKey not valid.");
            }

            if (profileReceiver == null)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "No profile found for recipent profile id.");
            }

            await _dataAccessLayer.InsertNotification(notif.RecipientProfileId, senderAppId, notif.Content);
        }

        public async Task<List<DBPlayerNotification>> GetAllPlayerNotifications(string apiKey)
        {
            var profile = await _dataAccessLayer.GetPlayerRecordByApiKey(apiKey);

            if (profile == null)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "No profile found matching apiKey provided.");
            }

            await _dataAccessLayer.EmptyPlayerNotifications(profile.ProfileId);

            return profile.PlayerNotifications;
        }
    }
}
