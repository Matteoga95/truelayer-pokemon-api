# TrueLayer Pokémon API
This project is a .NET 8 Web API that returns information about Pokémon, with
an option to translate their descriptions into fun versions (Shakespearean or
Yoda-like speech). It is built as a coding challenge for TrueLayer.
## Solution Overview
- **Pokemon.Api** – ASP.NET Core Web API project exposing endpoints to fetch
Pokémon details.
- **Pokemon.Core** – Class library for domain models and interfaces (the core
logic contracts).
- **Pokemon.Services** – Class library implementing business logic and
external API integrations.
- **Pokemon.Tests** – xUnit test project for unit and integration tests.


truelayer-pokemon-api (Solution)
├── Pokemon.sln
├── Pokemon.Api/
│ ├── Pokemon.Api.csproj
│ ├── Program.cs
│ └── Controllers/
│ └── (Removed WeatherForecast example files)
├── Pokemon.Core/
│ ├── Pokemon.Core.csproj
│ └── Class1.cs (empty placeholder class, safe to delete or ignore)
├── Pokemon.Services/
│ ├── Pokemon.Services.csproj
│ └── Class1.cs (empty placeholder class)
└── Pokemon.Tests/
 ├── Pokemon.Tests.csproj
 └── UnitTest1.cs (sample test class from template)
