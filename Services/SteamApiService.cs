using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SteamBotApi.Models;

namespace SteamBotApi.Services
{
    public class SteamApiService
    {
        private readonly HttpClient _httpClient;

        public SteamApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SteamBotApi/1.0");
        }

        public async Task<List<GameSearchResult>> SearchGamesByName(string name)
        {
            var url =
                $"https://store.steampowered.com/api/storesearch/?term={Uri.EscapeDataString(name)}&l=english&cc=UA";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("items").EnumerateArray();

            var results = new List<GameSearchResult>();
            foreach (var item in items)
            {
                if (
                    item.TryGetProperty("id", out JsonElement idElement)
                    && item.TryGetProperty("name", out JsonElement nameElement)
                )
                {
                    int id = idElement.GetInt32();
                    string? gameName = nameElement.GetString();
                    if (gameName != null)
                    {
                        results.Add(new GameSearchResult { Id = id, Name = gameName });
                        Console.WriteLine($"Game: ID={id}, Name={gameName}");
                        Console.WriteLine($"test");
                    }
                }
            }

            return results;
        }

        public async Task<Dictionary<string, object>?> GetGameDetails(int appId)
        {
            var url = $"https://store.steampowered.com/api/appdetails?appids={appId}";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            if (
                doc.RootElement.TryGetProperty(appId.ToString(), out JsonElement details)
                && details.GetProperty("success").GetBoolean()
            )
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(details.GetRawText());
            }
            return null;
        }
    }
}
