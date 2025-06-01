using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace SteamBotApi.Models
{
    public class UserSettings
    {
        [BsonId]
        public long ChatId { get; set; }

        public List<int> Wishlist { get; set; } = [];
        public bool SubscriptionOnSales { get; set; }
        public List<int> SubscribedGames { get; set; } = new List<int>();
    }
}
