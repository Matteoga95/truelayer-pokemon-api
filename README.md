# TrueLayer Pokémon API 🧪⚡

A **.NET 8** Web API that returns Pokémon info and can optionally translate the description into **Yoda** or **Shakespeare** style using the FunTranslations API.

---

## 🚀 Quick Launch (no clone required)

> You only need **Docker** installed. Choose one:

**1) Run the prebuilt image (one line):**
```bash
docker run --rm -p 8080:8080 matteoga95/truelayer-pokemon-api:latest
```
Open: **http://localhost:8080/swagger**

**2) Use Docker Compose directly from GitHub (no local files):**
```bash
docker compose -f https://raw.githubusercontent.com/matteoga95/truelayer-pokemon-api/main/docker-compose.yml up -d
```
Open: **http://localhost:8080/swagger**

> Using a paid FunTranslations plan? Set `ExternalApis__FunTranslations__ApiKey` (see **Configuration**).

---

## 🧭 Endpoints

| Method | Route | Description | Success | Errors |
|:------:|:----- |:----------- |:------:|:------ |
| GET | `/pokemon/{name}` | Pokémon info from PokéAPI (name, habitat, description, legendary). | 200 | 404 (unknown) |
| GET | `/pokemon/translated/{name}` | Same info, then translates description (**Yoda** if legendary or habitat = `cave`, else **Shakespeare**). | 200 | 404 (unknown), **502** (translation failed) |

---

## 🧱 Solution Structure

```
truelayer-pokemon-api.sln
├─ Pokemon.Api/           # ASP.NET Core Web API (Swagger)
│  ├─ Controllers/        # PokemonController
│  ├─ Dtos/               # API DTOs (optional)
│  ├─ Mappers/            # mapping helpers
│  ├─ appsettings.json    # config (no secrets)
│  └─ Program.cs
├─ Pokemon.Core/          # Domain & contracts
│  ├─ Interfaces/         # IPokeApiClient, IFunTranslationApiClient
│  └─ Models/             # PokemonInfo, FunTranslation, …
├─ Pokemon.Services/      # Implementations & external clients
│  ├─ ApiClients/         # PokeApiClient, FunTranslationApiClient
│  └─ Dtos/               # External API DTOs (internal)
└─ Pokemon.Tests/         # xUnit tests
   └─ ApiExternalTests.cs # External API Test ( PokeApi,  Fun Translation)
```

---

## ⚙️ Configuration

**`Pokemon.Api/appsettings.json` (safe defaults):**
```json
{
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  },
  "AllowedHosts": "*",
  "ExternalApis": {
    "PokeApi": {
      "BaseUrl": "https://pokeapi.co/api/v2/",
      "Endpoints": { "Species": "pokemon-species/{name}" }
    },
    "FunTranslations": {
      "BaseUrl": "https://api.funtranslations.com/translate/",
      "Endpoints": { "Yoda": "yoda.json", "Shakespeare": "shakespeare.json" },
      "ApiKeyHeader": "X-FunTranslations-Api-Secret",
      "ApiKey": "",
      "TimeoutSeconds": 10
    }
  }
}
```

**Environment variables (recommended for secrets):**
- `ExternalApis__FunTranslations__ApiKey` → your FunTranslations API key (leave empty for public/ratelimited plan).
- `ASPNETCORE_ENVIRONMENT` → `Development` (shows Swagger in container) or `Production`.

**Examples:**
```bash
# docker run
docker run --rm -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  matteoga95/truelayer-pokemon-api:latest
```

```yaml
# docker-compose.yml (with .env)
services:
  api:
    image: matteoga95/truelayer-pokemon-api:latest
    ports:
      - "8080:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
```

---

## 🐳 Docker

**Build locally:**
```bash
docker build -t truelayer-pokemon-api .
docker run --rm -p 8080:8080 truelayer-pokemon-api
# http://localhost:8080/swagger
```

**Compose (local build):**
```bash
docker compose up --build
# http://localhost:8080/swagger
```

> The image listens on **8080** and sets `ASPNETCORE_ENVIRONMENT=Development` in the container so Swagger is visible.

---

## 🧪 Tests

```bash
dotnet test
```
What’s covered:

- PokéAPI (happy path): verifies species data for a known Pokémon (e.g., pikachu) — non-empty description, non-null habitat, and IsLegendary == false.

- PokéAPI (unknown): calls with a clearly invalid name (e.g., definitely-not-a-pokemon) to assert the client returns null and the controller returns 404.

Translations (2 tests, strict):

- Shakespeare: requires success and a non-empty translated string for a sample English sentence.

- Yoda: requires success and a non-empty translated string for a sample English sentence.
---

## 🧠 Error Handling & Logging

The controller only returns **speaking, consistent errors** using `ProblemDetails`:

- **400 Bad Request** – invalid or empty Pokémon name.
- **404 Not Found** – Pokémon not found on PokéAPI.
- **502 Bad Gateway** – failed (PokéAPI/FunTranslations: errors, timeouts, or rate limits).

All errors include a clear **title** and **detail** message; 

---

## 🛠️ Develop From Source

**Prerequisites:** .NET 8 SDK (and optionally Docker)

```bash
git clone https://github.com/matteoga95/truelayer-pokemon-api.git
cd truelayer-pokemon-api
dotnet restore
dotnet run --project Pokemon.Api
# Swagger URL is printed in console (e.g., http://localhost:5221/swagger)
```

---

## 🔌 External APIs

- **PokéAPI** → species data (habitat, legendary flag, flavor text).
- **FunTranslations** → translation (`yoda.json`, `shakespeare.json`). Free tier is rate-limited.

---

## 🧼 Repo Hygiene

- `.gitignore` excludes build artifacts (`bin/`, `obj/`), IDE files (`.idea/`, `.vs/`, `*.user`), logs, and local-only secrets (`appsettings.Development.json`, `secrets.json`, `.env`).
- `.dockerignore` mirrors the above to keep Docker build contexts small.

---

## 📜 License

MIT (or your preferred license).

---

## 🙏 Credits

- Data by **PokéAPI**  
- Translations by **FunTranslations**
- Helper for this file By **ChatGpt**

---

### Handy Commands 🧩

```bash
# 1) Run prebuilt image
docker run --rm -p 8080:8080 matteoga95/truelayer-pokemon-api:latest

# 2) Compose directly from GitHub
docker compose -f https://raw.githubusercontent.com/matteoga95/truelayer-pokemon-api/main/docker-compose.yml up -d

# 3) Build from source → Docker
docker build -t truelayer-pokemon-api .
docker run --rm -p 8080:8080 truelayer-pokemon-api
```

Open **http://localhost:8080/swagger** 🎉
