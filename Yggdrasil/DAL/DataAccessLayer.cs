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
            var playerRecordMongoDbCollection = Environment.GetEnvironmentVariable("MONGODB_COLLECTION_NAME_PLAYERRECORD");

            var settings = MongoClientSettings.FromConnectionString(mongoDbConnectionString);
            _mongoClient = new MongoClient(settings);
            _database = _mongoClient.GetDatabase(mongoDbName);
            _playerRecordCollection = _database.GetCollection<PlayerRecordModel>(playerRecordMongoDbCollection);
        }

        public async Task<PlayerRecordModel> GetPlayerRecordByProfileId(string profileId)
        {
            IAsyncCursor<PlayerRecordModel> cursor = await _playerRecordCollection.FindAsync(doc => doc.ProfileId == profileId);
            var profile = await cursor.FirstOrDefaultAsync();

            return profile;
        }
        
        public async Task<PlayerRecordModel> GetPlayerRecordByEmail(string email)
        {
            IAsyncCursor<PlayerRecordModel> cursor = await _playerRecordCollection.FindAsync(doc => doc.Email == email);
            var profile = await cursor.FirstOrDefaultAsync();

            return profile;
        }
        
        public async Task<PlayerRecordModel> GetPlayerRecordByEmailAndPassword(string email, string password)
        {
            IAsyncCursor<PlayerRecordModel> cursor = await _playerRecordCollection.FindAsync(doc => ((doc.Email == email) && (doc.Password == password)));
            var profile = await cursor.FirstOrDefaultAsync();

            return profile;
        }

        public async Task InsertNotification(string recipientProfileId, string senderProfileId, string message)
        {
            DBPlayerNotification notif = new DBPlayerNotification()
            {
                SenderProfileId = senderProfileId,
                Content = message
            };

            var update = Builders<PlayerRecordModel>.Update
                .Push(doc => doc.PlayerNotifications, notif);

            await _playerRecordCollection.UpdateOneAsync(doc => doc.ProfileId == recipientProfileId, update);

            var update2 = Builders<PlayerRecordModel>.Update
                .Set(doc => doc.LastPnotSentTime, DateTime.UtcNow);

            await _playerRecordCollection.UpdateOneAsync(doc => doc.ProfileId == senderProfileId, update2);
        }

        public async Task EmptyPlayerNotifications(string profileId)
        {
            var update = Builders<PlayerRecordModel>.Update
                .Set(doc => doc.PlayerNotifications, new List<DBPlayerNotification>());

            await _playerRecordCollection.UpdateOneAsync(doc => doc.ProfileId == profileId, update);
        }

        public async Task<PlayerRecordModel> CreatePlayerRecord(PlayerRecordBaseInfo info)
        {
            PlayerRecordModel record = new PlayerRecordModel()
            {
                ProfileId = Guid.NewGuid().ToString(),
                Email = info.Email,
                Password = info.Password, //save only hash ??
                IsAdmin = false,
                PlayerNotifications = new List<DBPlayerNotification>(),
                LastPnotSentTime = DateTime.MinValue
            };

            await _playerRecordCollection.InsertOneAsync(record);
            return record;
        }

        public async Task DeletePlayerRecord(string profileId)
        {
            await _playerRecordCollection.DeleteOneAsync(doc => doc.ProfileId == profileId);
        }
    }
}
