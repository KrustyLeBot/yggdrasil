using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Yggdrasil.Models
{
    [BsonIgnoreExtraElements]
    public class PlayerRecordModel
    {
        [BsonRequired]
        public string ApiKey { get; set; }

        [BsonRequired]
        public string ProfileId { get; set; }

        [BsonRequired]
        public List<DBPlayerNotification> PlayerNotifications { get; set; }
    }
}
