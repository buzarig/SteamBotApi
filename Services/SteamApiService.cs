﻿using System.Net.Http;
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
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
            );
            _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("uk");
        }

        public async Task<List<GameSearchResult>> SearchGamesByName(string name)
        {
            var url =
                $"https://store.steampowered.com/api/storesearch/?term={Uri.EscapeDataString(name)}&l=ukrainian&cc=UA";
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

        public async Task<Dictionary<string, object>?> GetGameDetails(
            int appId,
            string countryCode = "UA",
            string language = "ukrainian"
        )
        {
            var url =
                $"https://store.steampowered.com/api/appdetails?appids={appId}&cc={countryCode}&l={language}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(request);
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

        public async Task<List<Dictionary<string, object>>> GetGameNewsAsync(
            int appId,
            int count = 1
        )
        {
            var url =
                $"https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid={appId}&count={count}&maxlength=300";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var newsItems = doc
                .RootElement.GetProperty("appnews")
                .GetProperty("newsitems")
                .EnumerateArray()
                .Select(x => JsonSerializer.Deserialize<Dictionary<string, object>>(x.GetRawText()))
                .ToList();

            return newsItems!;
        }
    }
}
