using MongoDB.Bson.Serialization.Attributes;

namespace Yggdrasil.Models
{
    [BsonIgnoreExtraElements]
    public class PlayerRecordModel
    {
        [BsonRequired]
        public string ApiKey { get; set; }
        [BsonRequired]
        public string ProfileId { get; set; }
    }
}
