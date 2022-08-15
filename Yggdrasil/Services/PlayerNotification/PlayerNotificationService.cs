using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Yggdrasil.DAL;
using Yggdrasil.HttpExceptions;
using Yggdrasil.Models;
using Yggdrasil.Services.PlayerRecord;

namespace Yggdrasil.Services.PlayerNotification
{
    public class PlayerNotificationService : IPlayerNotificationService
    {
        private readonly ILogger<PlayerNotificationService> _logger;
        private readonly IDataAccessLayer _dataAccessLayer;
        private readonly IPlayerRecordService _playerRecordService;

        public PlayerNotificationService(ILogger<PlayerNotificationService> logger, IDataAccessLayer dataAccessLayer, IPlayerRecordService playerRecordService)
        {
            _logger = logger;
            _dataAccessLayer = dataAccessLayer;
            _playerRecordService = playerRecordService;
        }

        public async Task SendPlayerNotification(string senderProfileId, PlayerNotificationModel notif)
        {
            PlayerRecordInternalModel profileSender = await _playerRecordService.InternalGetPlayerRecordByProfileId(senderProfileId);
            PlayerRecordInternalModel profileReceiver = await _playerRecordService.InternalGetPlayerRecordByProfileId(notif.RecipientProfileId);

            if (profileSender == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No profile found matching sender profile id provided.");
            }

            if (profileReceiver == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No profile found for recipent profile id.");
            }

            if(profileSender.LastPnotSentTime.AddSeconds(30) > DateTime.UtcNow)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "A pnot was already sent during the last 30 second.");
            }

            await _dataAccessLayer.InsertNotification(notif.RecipientProfileId, profileSender.ProfileId, notif.Content);
        }

        public async Task SendPlayerNotificationAdmin(PlayerNotificationModel notif, string senderAppId)
        {
            PlayerRecordInternalModel callerProfile = await _playerRecordService.InternalGetPlayerRecordByProfileId(senderAppId);

            if (callerProfile == null || !callerProfile.IsAdmin)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Caller does not have admin status.");
            }

            PlayerRecordInternalModel profileReceiver = await _playerRecordService.InternalGetPlayerRecordByProfileId(notif.RecipientProfileId);

            if (profileReceiver == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No profile found for recipent profile id.");
            }

            await _dataAccessLayer.InsertNotification(notif.RecipientProfileId, senderAppId, notif.Content);
        }

        public async Task<List<DBPlayerNotification>> GetAllPlayerNotifications(string profileId)
        {
            PlayerRecordInternalModel profile = await _playerRecordService.InternalGetPlayerRecordByProfileId(profileId);

            if (profile == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "No profile found matching profile id provided.");
            }

            await _dataAccessLayer.EmptyPlayerNotifications(profile.ProfileId);

            return profile.PlayerNotifications;
        }
    }
}
