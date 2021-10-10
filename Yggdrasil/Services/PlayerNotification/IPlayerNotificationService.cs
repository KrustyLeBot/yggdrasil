using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.Services.PlayerNotification
{
    public interface IPlayerNotificationService
    {
        public Task<int> CreatePlayerSocket(string apiKey, WebSocketManager webSockets);
        public Task<int> SendPlayerNotification(string apiKey, PlayerNotificationModel notif);
    }
}
