using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Yggdrasil.Models
{
    [BsonIgnoreExtraElements]
    public class PlayerRecordModel
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
    }

    public class PlayerRecordBaseInfo
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
