using Microsoft.AspNetCore.Mvc;
using SteamBotApi.Models;
using SteamBotApi.Services;
using SteamStoreBot.Services;

namespace SteamBotApi.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly SteamApiService _steamApiService;
        private readonly SteamSpyClient _steamSpyClient;

        public SearchController(SteamApiService steamApiService, SteamSpyClient steamSpyClient)
        {
            _steamApiService = steamApiService;
            _steamSpyClient = steamSpyClient;
        }

        [HttpGet("games")]
        public async Task<ActionResult<List<GameSearchResult>>> SearchGames(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required");
            }

            var games = await _steamApiService.SearchGamesByName(name);
            if (!games.Any())
            {
                return NotFound("No games found");
            }
            return Ok(games);
        }

        [HttpGet("details")]
        public async Task<ActionResult<Dictionary<string, object>?>> GetGameDetails(
            int appId,
            string cc = "UA",
            string l = "ukrainian"
        )
        {
            var details = await _steamApiService.GetGameDetails(appId, cc, l);
            if (details == null)
                return NotFound();
            return Ok(details);
        }

        [HttpGet("spy-genre")]
        public async Task<ActionResult<List<GameSearchResult>>> GetFromSpyByGenre(
            [FromQuery] string genre,
            [FromQuery] int minRating = 0,
            [FromQuery] int minVotes = 10
        )
        {
            var spyGames = await _steamSpyClient.GetGamesByGenreAsync(genre);

            var result = spyGames
                .Values.Where(g => g.Positive + g.Negative >= minVotes)
                .Select(g =>
                {
                    var total = g.Positive + g.Negative;
                    var rating = total > 0 ? g.Positive * 100 / total : 0;

                    return new GameSearchResult
                    {
                        Id = g.Appid,
                        Name = g.Name,
                        Discount = g.Discount ?? 0,
                        Rating = rating,
                    };
                })
                .Where(g => g.Rating >= minRating)
                .OrderByDescending(g => g.Rating)
                .ThenByDescending(g => g.Discount)
                .Take(20)
                .ToList();

            return Ok(result);
        }

        [HttpGet("spy-budget")]
        public async Task<ActionResult<List<GameSearchResult>>> GetFromSpyByBudget(
            [FromQuery] double max,
            [FromQuery] int minRating = 0,
            [FromQuery] int minVotes = 0
        )
        {
            if (max <= 0)
                return BadRequest("Бюджет має бути більше 0");

            var all = await _steamSpyClient.GetAllGamesAsync();

            var paidGames = all
                .Values.Where(g => g.Price > 0 && g.Price <= max * 100)
                .Where(g => g.Positive + g.Negative >= minVotes)
                .Select(g =>
                {
                    var total = g.Positive + g.Negative;
                    var rating = total > 0 ? g.Positive * 100 / total : 0;

                    return new GameSearchResult
                    {
                        Id = g.Appid,
                        Name = g.Name,
                        Discount = g.Discount ?? 0,
                        Rating = rating,
                        Price = g.Price,
                    };
                })
                .Where(g => g.Rating >= minRating)
                .OrderByDescending(g => g.Price)
                .Take(30)
                .ToList();

            if (paidGames.Any())
                return Ok(paidGames);

            var freeGames = all
                .Values.Where(g => g.Price == 0)
                .Where(g => g.Positive + g.Negative >= minVotes)
                .Select(g =>
                {
                    var total = g.Positive + g.Negative;
                    var rating = total > 0 ? g.Positive * 100 / total : 0;

                    return new GameSearchResult
                    {
                        Id = g.Appid,
                        Name = g.Name,
                        Discount = g.Discount ?? 0,
                        Rating = rating,
                        Price = 0,
                    };
                })
                .Where(g => g.Rating >= minRating)
                .OrderByDescending(g => g.Rating)
                .Take(15)
                .ToList();

            return Ok(freeGames);
        }

        [HttpGet("spy-discounts")]
        public async Task<ActionResult<List<GameSearchResult>>> GetDiscountedGames()
        {
            var all = await _steamSpyClient.GetAllGamesAsync();

            var discountedGames = all
                .Values.Where(g => g.Price > 0 && g.Discount > 0)
                .OrderByDescending(g => g.Discount)
                .Take(10)
                .Select(g => new GameSearchResult
                {
                    Id = g.Appid,
                    Name = g.Name,
                    Discount = g.Discount ?? 0,
                })
                .ToList();

            return Ok(discountedGames);
        }

        [HttpGet("news")]
        public async Task<ActionResult<List<Dictionary<string, object>>>> GetGameNews(
            [FromQuery] int appId
        )
        {
            var news = await _steamApiService.GetGameNewsAsync(appId, 1);
            return Ok(news);
        }
    }
}
