#####################################################
# 1. Build stage – компілюємо ваш .NET-проєкт
#####################################################
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копіюємо файл з проектом (csproj) у робочу теку /src
COPY SteamBotApi.csproj ./

# Відновлюємо залежності лише для цього .csproj
RUN dotnet restore ./SteamBotApi.csproj

# Копіюємо решту всіх файлів у /src (код, контролери, моделі, appsettings тощо)
COPY . .

# Переходимо в теку з .csproj і будуємо у Release у папку /app/publish
WORKDIR /src
RUN dotnet publish -c Release -o /app/publish

#####################################################
# 2. Runtime stage – беремо лише runtime-образ
#####################################################
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Копіюємо з build-стадії згенеровані файли у фінальний образ
COPY --from=build /app/publish ./

# При старті контейнера виконуємо цей ENTRYPOINT
ENTRYPOINT ["dotnet", "SteamBotApi.dll"]
