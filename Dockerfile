# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia sln e csproj per usare la cache del restore
COPY truelayer-pokemon-api.sln ./
COPY Pokemon.Api/Pokemon.Api.csproj Pokemon.Api/
COPY Pokemon.Core/Pokemon.Core.csproj Pokemon.Core/
COPY Pokemon.Services/Pokemon.Services.csproj Pokemon.Services/
COPY Pokemon.Tests/Pokemon.Tests.csproj Pokemon.Tests/

RUN dotnet restore ./Pokemon.Api/Pokemon.Api.csproj

# Copia il resto e pubblica
COPY . .
RUN dotnet publish ./Pokemon.Api/Pokemon.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# L'API ascolta su 8080
ENV ASPNETCORE_URLS=http://+:8080
# In container abilito Swagger (se nel Program.cs è messo solo per Development)
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Pokemon.Api.dll"]
