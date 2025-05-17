using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SteamBotApi.Models;
using SteamBotApi.Services;

namespace SteamBotApi.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly SteamApiService _steamApiService;

        public SearchController(SteamApiService steamApiService)
        {
            _steamApiService = steamApiService;
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
        public async Task<ActionResult<Dictionary<string, object>?>> GetGameDetails(int appId)
        {
            var details = await _steamApiService.GetGameDetails(appId);
            if (details == null)
            {
                return NotFound("Game details not found");
            }
            return Ok(details);
        }
    }
}
