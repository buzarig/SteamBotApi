using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SteamBotApi.MongoDB;
using SteamBotApi.Services;

var builder = WebApplication.CreateBuilder(args);

var mongoSection = builder.Configuration.GetSection("MongoDbSettings");
var connectionString = mongoSection.GetValue<string>("ConnectionString");
var databaseName = mongoSection.GetValue<string>("DatabaseName");
if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
    throw new InvalidOperationException(
        "MongoDB connection settings are missing in appsettings.json"
    );

builder.Services.AddSingleton(sp => new MongoDbContext(connectionString, databaseName));

builder.Services.AddHttpClient<SteamApiService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
