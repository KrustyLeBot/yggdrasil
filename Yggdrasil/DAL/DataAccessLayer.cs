using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.DAL
{
    public class DataAccessLayer : IDataAccessLayer
    {
        private readonly ILogger<DataAccessLayer> _logger;

        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        public readonly IMongoCollection<PlayerRecordModel> _playerRecordCollection;

        public DataAccessLayer(ILogger<DataAccessLayer> logger)
        {
            _logger = logger;

            var mongoDbConnectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            var mongoDbName = Environment.GetEnvironmentVariable("MONGODB_DB_NAME");
            var mongoDbCollection = Environment.GetEnvironmentVariable("MONGODB_COLLECTION_NAME");

            var settings = MongoClientSettings.FromConnectionString(mongoDbConnectionString);
            _mongoClient = new MongoClient(settings);
            _database = _mongoClient.GetDatabase(mongoDbName);
            _playerRecordCollection = _database.GetCollection<PlayerRecordModel>(mongoDbCollection);
        }

        public async Task<PlayerRecordModel> GetPlayerRecord(string apiKey)
        {
            IAsyncCursor<PlayerRecordModel> cursor = await _playerRecordCollection.FindAsync(doc => doc.ApiKey == apiKey);
            var profile = await cursor.FirstOrDefaultAsync();

            return profile;
        }

        public async Task InsertOfflineNotification(string recipientProfileId, string senderProfileId, string message)
        {
            OfflinePlayerNotification notif = new OfflinePlayerNotification()
            {
                SenderProfileId = senderProfileId,
                Content = message
            };

            var update = Builders<PlayerRecordModel>.Update
                .Push(doc => doc.OfflineNotifications, notif);

            await _playerRecordCollection.UpdateOneAsync(doc => doc.ProfileId == recipientProfileId, update);
        }

        public async Task EmptyOfflineNotification(string profileId)
        {
            var update = Builders<PlayerRecordModel>.Update
                .Set(doc => doc.OfflineNotifications, new List<OfflinePlayerNotification>());

            await _playerRecordCollection.UpdateOneAsync(doc => doc.ProfileId == profileId, update);
        }
    }
}
