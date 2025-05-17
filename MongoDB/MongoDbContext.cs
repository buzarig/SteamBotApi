using MongoDB.Driver;
using SteamBotApi.Models;

namespace SteamBotApi.MongoDB
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(
                    nameof(connectionString),
                    "Connection string cannot be null or empty"
                );
            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException(
                    nameof(databaseName),
                    "Database name cannot be null or empty"
                );

            try
            {
                var client = new MongoClient(connectionString);
                _database = client.GetDatabase(databaseName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to MongoDB: {ex.Message}");
                throw;
            }
        }

        public IMongoCollection<UserSettings> Users =>
            _database.GetCollection<UserSettings>("users");
    }
}
