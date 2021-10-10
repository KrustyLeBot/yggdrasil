using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.DAL;
using Yggdrasil.Models;

namespace Yggdrasil.Services.PlayerNotification
{
    public class PlayerNotificationService : IPlayerNotificationService
    {
        private readonly ILogger<PlayerNotificationService> _logger;
        private readonly IDataAccessLayer _dataAccessLayer;

        private Dictionary<string, WebSocket> _websocketList;

        public PlayerNotificationService(ILogger<PlayerNotificationService> logger, IDataAccessLayer dataAccessLayer)
        {
            _logger = logger;
            _dataAccessLayer = dataAccessLayer;

            _websocketList = new Dictionary<string, WebSocket>();
        }

        public async Task<int> CreatePlayerSocket(string apiKey, WebSocketManager webSockets)
        {
            var profile = await _dataAccessLayer.GetPlayerRecord(apiKey);

            if(profile == null)
            {
                return 401;
            }

            if(_websocketList.ContainsKey(profile.ProfileId))
            {
                return 409;
            }

            var webSocket = await webSockets.AcceptWebSocketAsync();

            // register the messaging queue
            _websocketList.Add(profile.ProfileId, webSocket);

            _logger.Log(LogLevel.Information, "WebSocket connection established");

            await KeepAlive(webSocket, profile.ProfileId);
            return 200;
        }

        public async Task<int> SendPlayerNotification(string apiKey, PlayerNotificationModel notif)
        {
            var profile = await _dataAccessLayer.GetPlayerRecord(apiKey);

            if (profile == null)
            {
                return 401;
            }

            if (_websocketList.TryGetValue(notif.RecipientProfileId, out WebSocket webSocket))
            {
                var message = Encoding.UTF8.GetBytes($"Server: Hello. {profile.ProfileId} sent you: {notif.Content}");
                await webSocket.SendAsync(new ArraySegment<byte>(message, 0, message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                _logger.Log(LogLevel.Information, "Message sent to Client");

                return 200;
            }


            if(notif.IsOffline)
            {
                // save msg in db
                return 201;
            }

            return 410;
        }

        private async Task KeepAlive(WebSocket webSocket, string profileId)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            _logger.Log(LogLevel.Information, "Message received from Client");

            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                _logger.Log(LogLevel.Information, "Message received from Client");

            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _websocketList.Remove(profileId);
            _logger.Log(LogLevel.Information, "WebSocket connection closed");
        }
    }
}
