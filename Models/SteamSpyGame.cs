using System.Text.Json.Serialization;

namespace SteamBotApi.Models
{
    public class SteamSpyGame
    {
        public int Appid { get; set; }
        public required string Name { get; set; }
        public int Positive { get; set; }
        public int Negative { get; set; }
        public required string Developer { get; set; }
        public required string Publisher { get; set; }
        public string? ScoreRank { get; set; }
        public string Owners { get; set; } = "";
        public int AverageForever { get; set; }
        public int Average2Weeks { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int Price { get; set; }

        public string InitialPrice { get; set; } = "";
        public int? Discount { get; set; }
        public string Tags { get; set; } = "";
        public string Languages { get; set; } = "";
        public string Genre { get; set; } = "";
        public string ReleaseDate { get; set; } = "";
    }
}
