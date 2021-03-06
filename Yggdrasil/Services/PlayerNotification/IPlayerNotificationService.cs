using System.Collections.Generic;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.Services.PlayerNotification
{
    public interface IPlayerNotificationService
    {
        public Task SendPlayerNotification(string apiKey, PlayerNotificationModel notif);
        public Task SendPlayerNotificationAdmin(string apiKey, PlayerNotificationModel notif, string senderAppId);
        public Task<List<DBPlayerNotification>> GetAllPlayerNotifications(string apiKey);
    }
}
