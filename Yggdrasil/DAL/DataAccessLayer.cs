using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.DAL
{
    public class DataAccessLayer : IDataAccessLayer
    {
        private readonly ILogger<DataAccessLayer> _logger;

        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        public readonly IMongoCollection<PlayerRecordDBModel> _playerRecordCollection;
        public readonly IMongoCollection<ItemModel> _itemStoreCollection;

        public DataAccessLayer(ILogger<DataAccessLayer> logger)
        {
            _logger = logger;

            var mongoDbConnectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            var mongoDbName = Environment.GetEnvironmentVariable("MONGODB_DB_NAME");
            var playerRecordMongoDbCollection = Environment.GetEnvironmentVariable("MONGODB_COLLECTION_NAME_PLAYERRECORD");
            var itemStoreMongoDbCollection = Environment.GetEnvironmentVariable("MONGODB_COLLECTION_NAME_ITEMSTORE");

            var settings = MongoClientSettings.FromConnectionString(mongoDbConnectionString);
            _mongoClient = new MongoClient(settings);
            _database = _mongoClient.GetDatabase(mongoDbName);
            _playerRecordCollection = _database.GetCollection<PlayerRecordDBModel>(playerRecordMongoDbCollection);
            _itemStoreCollection = _database.GetCollection<ItemModel>(itemStoreMongoDbCollection);
        }

        public async Task<PlayerRecordInternalModel> GetPlayerRecordByProfileId(string profileId)
        {
            IAsyncCursor<PlayerRecordDBModel> cursor = await _playerRecordCollection.FindAsync(doc => doc.ProfileId == profileId);
            var profile = await cursor.FirstOrDefaultAsync();
            
            return profile;
        }
        
        public async Task<PlayerRecordInternalModel> GetPlayerRecordByEmail(string email)
        {
            IAsyncCursor<PlayerRecordDBModel> cursor = await _playerRecordCollection.FindAsync(doc => doc.Email == email);
            var profile = await cursor.FirstOrDefaultAsync();

            return profile;
        }
        
        public async Task<PlayerRecordInternalModel> GetPlayerRecordByEmailAndPassword(string email, string password)
        {
            IAsyncCursor<PlayerRecordDBModel> cursor = await _playerRecordCollection.FindAsync(doc => ((doc.Email == email) && (doc.Password == sha256(password))));
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

            var update = Builders<PlayerRecordDBModel>.Update
                .Push(doc => doc.PlayerNotifications, notif);

            await _playerRecordCollection.UpdateOneAsync(doc => doc.ProfileId == recipientProfileId, update);

            var update2 = Builders<PlayerRecordDBModel>.Update
                .Set(doc => doc.LastPnotSentTime, DateTime.UtcNow);

            await _playerRecordCollection.UpdateOneAsync(doc => doc.ProfileId == senderProfileId, update2);
        }

        public async Task EmptyPlayerNotifications(string profileId)
        {
            var update = Builders<PlayerRecordDBModel>.Update
                .Set(doc => doc.PlayerNotifications, new List<DBPlayerNotification>());

            await _playerRecordCollection.UpdateOneAsync(doc => doc.ProfileId == profileId, update);
        }

        public async Task<PlayerRecordInternalModel> CreatePlayerRecord(PlayerRecordBaseInfo info)
        {
            PlayerRecordDBModel record = new PlayerRecordDBModel()
            {
                ProfileId = Guid.NewGuid().ToString(),
                Email = info.Email,
                Password = sha256(info.Password),
                IsAdmin = false,
                PlayerNotifications = new List<DBPlayerNotification>(),
                LastPnotSentTime = DateTime.MinValue,
                Inventory = new List<InventoryItemModel>()
            };

            await _playerRecordCollection.InsertOneAsync(record);

            return record;
        }

        public async Task DeletePlayerRecord(string profileId)
        {
            await _playerRecordCollection.DeleteOneAsync(doc => doc.ProfileId == profileId);
        }

        public async Task<List<ItemModel>> GetItemStore()
        {
            var items = await _itemStoreCollection.FindAsync(_ => true);
            return items.ToList();
        }

        public async Task InsertItem(ItemModel item)
        {
            await _itemStoreCollection.ReplaceOneAsync(doc => doc.ItemId == item.ItemId, item, new ReplaceOptions { IsUpsert = true });
        }

        public async Task GrantItems(GrantedItems info)
        {
            Dictionary<string, int> maxQuantityByItemId = new Dictionary<string, int>();

            for (int i = info.GrantedItemsList.Count - 1; i >= 0; i--)
            {
                IAsyncCursor<ItemModel> cursor = await _itemStoreCollection.FindAsync(doc => doc.ItemId == info.GrantedItemsList[i].ItemId);
                var item = await cursor.FirstOrDefaultAsync();

                if (item == null)
                {
                    info.GrantedItemsList.RemoveAt(i);
                    continue;
                }

                maxQuantityByItemId[item.ItemId] = item.MaxQuantity;
            }

            List<Task> tasksAddToSet = new List<Task>();
            List<Task> tasksUpdateQuantity = new List<Task>();

            foreach (GrantedItem grant in info.GrantedItemsList)
            {
                //Set item quantity to 0 for player not having item already
                {
                    var queryBuilder = Builders<PlayerRecordDBModel>.Filter;
                    var elemMatchBuilder = Builders<InventoryItemModel>.Filter;

                    var filter = queryBuilder.Eq(document => document.ProfileId, grant.ProfileId) & (queryBuilder.ElemMatch(document => document.Inventory, elemMatchBuilder.Ne(document => document.ItemId, grant.ItemId)) | queryBuilder.Eq(document => document.Inventory, new List<InventoryItemModel>()));

                    InventoryItemModel inventoryItem = new InventoryItemModel()
                    {
                        ItemId = grant.ItemId,
                        Quantity = 0
                    };

                    var update = Builders<PlayerRecordDBModel>.Update
                        .AddToSet(x => x.Inventory, inventoryItem);

                    tasksAddToSet.Add(_playerRecordCollection.FindOneAndUpdateAsync(filter, update));
                }

                //Increment quantity for player not going above max quantity
                {
                    var queryBuilder = Builders<PlayerRecordDBModel>.Filter;
                    var elemMatchBuilder = Builders<InventoryItemModel>.Filter;

                    var filter = queryBuilder.Eq(document => document.ProfileId, grant.ProfileId) & queryBuilder.ElemMatch(document => document.Inventory, elemMatchBuilder.Eq(document => document.ItemId, grant.ItemId)) & queryBuilder.ElemMatch(document => document.Inventory, elemMatchBuilder.Lte(document => document.Quantity, maxQuantityByItemId[grant.ItemId] - grant.Quantity));

                    var update = Builders<PlayerRecordDBModel>.Update
                        .Inc(x => x.Inventory[-1].Quantity, grant.Quantity);

                    tasksUpdateQuantity.Add(_playerRecordCollection.UpdateOneAsync(filter, update));
                }

                //Set quantity to max quantity for player going above max quantity
                {
                    var queryBuilder = Builders<PlayerRecordDBModel>.Filter;
                    var elemMatchBuilder = Builders<InventoryItemModel>.Filter;

                    var filter = queryBuilder.Eq(document => document.ProfileId, grant.ProfileId) & queryBuilder.ElemMatch(document => document.Inventory, elemMatchBuilder.Eq(document => document.ItemId, grant.ItemId)) & queryBuilder.ElemMatch(document => document.Inventory, elemMatchBuilder.Gt(document => document.Quantity, maxQuantityByItemId[grant.ItemId] - grant.Quantity));

                    var update = Builders<PlayerRecordDBModel>.Update
                        .Set(x => x.Inventory[-1].Quantity, maxQuantityByItemId[grant.ItemId]);

                    tasksUpdateQuantity.Add(_playerRecordCollection.UpdateOneAsync(filter, update));
                }
            }

            await Task.WhenAll(tasksAddToSet);
            await Task.WhenAll(tasksUpdateQuantity);
        }

        static string sha256(string randomString)
        {
            var crypt = SHA256.Create();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
