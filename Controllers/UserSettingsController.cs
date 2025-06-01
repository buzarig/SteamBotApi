using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SteamBotApi.Models;
using SteamBotApi.MongoDB;

namespace SteamBotApi.Controllers
{
    [Route("api/userSettings")]
    [ApiController]
    public class UserSettingsController : ControllerBase
    {
        private readonly MongoDbContext _db;
        private readonly ILogger<UserSettingsController> _logger;

        public UserSettingsController(MongoDbContext db, ILogger<UserSettingsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // api/userSettings/chatId
        [HttpGet("{chatId}")]
        public async Task<ActionResult<UserSettings>> Get(long chatId)
        {
            _logger.LogInformation("Get UserSettings for ChatId={ChatId}", chatId);
            var user = await _db.Users.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
            if (user == null)
            {
                _logger.LogWarning("UserSettings not found for ChatId={ChatId}", chatId);
                return NotFound();
            }
            return Ok(user);
        }

        // api/userSettings/chatId
        [HttpPut("{chatId}")]
        public async Task<IActionResult> Put(long chatId, [FromBody] UserSettings settings)
        {
            _logger.LogInformation("Upsert UserSettings for ChatId={ChatId}", chatId);
            await _db.Users.ReplaceOneAsync(
                u => u.ChatId == chatId,
                settings,
                new ReplaceOptions { IsUpsert = true }
            );
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<UserSettings>>> GetAllUsers()
        {
            var users = await _db.Users.Find(_ => true).ToListAsync();
            return Ok(users);
        }
    }
}
