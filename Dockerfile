#############################################
# 1. Build stage – компілюємо ваш .NET 8-проєкт
#############################################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копіюємо файл .csproj у робочу теку /src
COPY SteamBotApi.csproj ./

# Відновлюємо залежності для цього .csproj (тепер це .NET 8)
RUN dotnet restore ./SteamBotApi.csproj

# Копіюємо всі файли проєкту у /src
COPY . .

# Базова робоча тека всередині контейнера – /src
WORKDIR /src

# Збираємо проєкт у Release у папку /app/publish
RUN dotnet publish -c Release -o /app/publish

#############################################
# 2. Runtime stage – використовуємо .NET 8 runtime
#############################################
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копіюємо з build-стадії згенеровані файли у фінальну /app
COPY --from=build /app/publish ./

# Запускаємо зібраний DLL
ENTRYPOINT ["dotnet", "SteamBotApi.dll"]
