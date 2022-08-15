using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Yggdrasil.Models
{
    [BsonIgnoreExtraElements]
    public class PlayerRecordDBModel
    {
        [BsonRequired]
        public string Email { get; set; }

        [BsonRequired]
        public string Password { get; set; }

        [BsonRequired]
        public string ProfileId { get; set; }
        
        [BsonRequired]
        public bool IsAdmin { get; set; }

        [BsonRequired]
        public List<DBPlayerNotification> PlayerNotifications { get; set; }

        [BsonRequired]
        public DateTime LastPnotSentTime { get; set; }
        
        [BsonRequired]
        public List<InventoryItemModel> Inventory { get; set; }
    }

    public class PlayerRecordInternalModel
    {
        public string Email { get; set; }
        public string ProfileId { get; set; }
        public bool IsAdmin { get; set; }
        public List<DBPlayerNotification> PlayerNotifications { get; set; }
        public DateTime LastPnotSentTime { get; set; }
        public List<InventoryItemModel> Inventory { get; set; }

        public static implicit operator PlayerRecordInternalModel(PlayerRecordDBModel x)
        {
            PlayerRecordInternalModel record = new PlayerRecordInternalModel();
            record.Email = x.Email;
            record.ProfileId = x.ProfileId;
            record.IsAdmin = x.IsAdmin;
            record.PlayerNotifications = x.PlayerNotifications;
            record.LastPnotSentTime = x.LastPnotSentTime;
            record.Inventory = x.Inventory;
            return record;
        }
    }

    public class PlayerRecordRVModel
    {
        public string Email { get; set; }
        public string ProfileId { get; set; }

        public static implicit operator PlayerRecordRVModel(PlayerRecordInternalModel x)
        {
            PlayerRecordRVModel record = new PlayerRecordRVModel();
            record.Email = x.Email;
            record.ProfileId = x.ProfileId;
            return record;
        }
    }

    public class PlayerRecordBaseInfo
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
