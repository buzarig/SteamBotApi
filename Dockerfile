#############################################
# 1. Build stage: ��������� ����� � ����� /app/publish
#############################################
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# ������� ���� csproj, ��� ������ �������� ���������
COPY SteamBotApi/SteamBotApi.csproj ./SteamBotApi/
RUN dotnet restore SteamBotApi/SteamBotApi.csproj

# ������� ����� ����� � ������ �����
COPY SteamBotApi/. ./SteamBotApi/
WORKDIR /src/SteamBotApi
RUN dotnet publish -c Release -o /app/publish

#############################################
# 2. Runtime stage: ������ ����� ������� Release
#############################################
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# ��� ������� ���������� ���� �������� �� �������
ENTRYPOINT ["dotnet", "SteamBotApi.dll"]
