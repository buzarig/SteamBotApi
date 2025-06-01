using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SteamBotApi.Models;

namespace SteamStoreBot.Services
{
    public class SteamSpyClient
    {
        private readonly HttpClient _httpClient;

        public SteamSpyClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://steamspy.com/api.php");
        }

        public async Task<Dictionary<string, SteamSpyGame>> GetGamesByGenreAsync(string genre)
        {
            var genreMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "mmo", "Massively Multiplayer" },
                { "рпг", "RPG" },
                { "рольові", "RPG" },
                { "екшн", "Action" },
                { "бойовики", "Action" },
                { "пригоди", "Adventure" },
                { "інді", "Indie" },
                { "стратегія", "Strategy" },
                { "симулятор", "Simulation" },
                { "перегони", "Racing" },
                { "спортивні", "Sports" },
                { "жахи", "Horror" },
                { "головоломки", "Puzzle" },
            };

            if (genreMap.TryGetValue(genre.Trim().ToLower(), out var normalized))
                genre = normalized;

            var url = $"?request=genre&genre={Uri.EscapeDataString(genre)}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, SteamSpyGame>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
                }
            );

            return result ?? new();
        }

        public async Task<Dictionary<string, SteamSpyGame>> GetAllGamesAsync()
        {
            var response = await _httpClient.GetAsync("?request=all");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, SteamSpyGame>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
                }
            );

            return result ?? new();
        }
    }
}
