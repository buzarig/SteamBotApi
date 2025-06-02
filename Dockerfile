#############################################
# 1. Build stage: компілюємо проєкт у папку /app/publish
#############################################
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копіюємо лише csproj, щоб швидше кешувати залежності
COPY SteamBotApi/SteamBotApi.csproj ./SteamBotApi/
RUN dotnet restore SteamBotApi/SteamBotApi.csproj

# Копіюємо решту файлів і робимо пабліш
COPY SteamBotApi/. ./SteamBotApi/
WORKDIR /src/SteamBotApi
RUN dotnet publish -c Release -o /app/publish

#############################################
# 2. Runtime stage: беремо тільки зібраний Release
#############################################
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# При запуску контейнера буде виконана ця команда
ENTRYPOINT ["dotnet", "SteamBotApi.dll"]
