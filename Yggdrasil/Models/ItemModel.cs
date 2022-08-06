using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Text.Json;

namespace Yggdrasil.Models
{
    [BsonIgnoreExtraElements]
    public class ItemModel
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public int MaxQuantity { get; set; }
        public string Obj  { get; set; }
}

    public class InventoryItemModel
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class GrantedItem
    {
        public string ProfileId { get; set; }
        public string ItemId { get; set; }
        public int Quantity { get; set; }
    }
    
    public class GrantedItems
    {
        public List<GrantedItem> GrantedItemsList { get; set; }
    }
}
