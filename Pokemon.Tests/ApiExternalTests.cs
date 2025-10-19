using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pokemon.Core.Interfaces;
using Pokemon.Services.ApiClients;
using Xunit;

namespace Pokemon.Tests;

public sealed class TestFixture : IDisposable
{
    public IServiceProvider Services { get; }

    public TestFixture()
    {
        // Build in-memory configuration (mirrors appsettings.json structure)
        var configData = new Dictionary<string, string?>
        {
            ["ExternalApis:PokeApi:BaseUrl"] = "https://pokeapi.co/api/v2/",
            ["ExternalApis:PokeApi:Endpoints:Species"] = "pokemon-species/{name}",
            ["ExternalApis:FunTranslation:BaseUrl"] = "https://api.funtranslations.com/translate/",
            ["ExternalApis:FunTranslation:Endpoints:Yoda"] = "yoda.json",
            ["ExternalApis:FunTranslation:Endpoints:Shakespeare"] = "shakespeare.json",
            ["ExternalApis:FunTranslation:ApiKeyHeader"] = "X-FunTranslations-Api-Secret",
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        
        var pokeBase = configuration["ExternalApis:PokeApi:BaseUrl"]!;
        services.AddHttpClient<IPokeApiClient, PokeApiClient>(c =>
        {
            c.BaseAddress = new Uri(pokeBase);
            c.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddHttpClient<IFunTranslationApiClient, FunTranslationApiClient>(c =>
        {
            c.Timeout = TimeSpan.FromSeconds(10);
        });

        Services = services.BuildServiceProvider();
    }
    
    public void Dispose()
    {
        if (Services is IDisposable d) d.Dispose();
    }
}

public class ApiExternalTests(TestFixture fx) : IClassFixture<TestFixture>
{
    private readonly IPokeApiClient _poke = fx.Services.GetRequiredService<IPokeApiClient>();
    private readonly IFunTranslationApiClient _fun = fx.Services.GetRequiredService<IFunTranslationApiClient>();

    [Fact(DisplayName = "PokeApi: Get pikachu")]
    public async Task PokeApi_Pikachu_ShouldReturnSpeciesData()
    {
        try
        {
            var res = await _poke.GetPokemonInfoAsync("pikachu");

            res.Should().NotBeNull();
            res!.Name.Should().Be("pikachu");
            res.Description.Should().NotBeNullOrWhiteSpace();
            res.Habitat.Should().NotBeNull();    
            res.IsLegendary.Should().BeFalse();      
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception while calling PokéAPI: {ex.GetType().Name}: {ex.Message}");
        }
    }
    
    [Fact(DisplayName = "PokeApi: (INTENTIONAL FAIL)")]
    public async Task PokeApi_Unknown_ShouldFailTest()
    {
        try
        {
            var res = await _poke.GetPokemonInfoAsync("definitely-not-a-pokemon");
            
            res.Should().BeNull("this test is meant to pass when species is not found");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception while calling PokéAPI: {ex.GetType().Name}: {ex.Message}");
        }
    }

    [Fact(DisplayName = "FunTranslations: Shakespeare translation ")]
    public async Task FunTranslations_Shakespeare_ShouldReturnOrGracefullyFail()
    {
        try
        {
            // sample from Pikachu 
            const string text = "When several of these POKéMON gather, their electricity could build and cause lightning storms.";

            var res = await _fun.GetShakespeareTranslationAsync(text);

            res.Should().NotBeNull();
            res!.Text.Should().NotBeNullOrWhiteSpace();

            if (res.isSuccess)
            {
                res.Translation.Should().NotBeNullOrWhiteSpace("Translation is null, Api server error or rate limiter error");
            }

        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception during Shakespeare translation: {ex.GetType().Name}: {ex.Message}");
        }
    }

    [Fact(DisplayName = "FunTranslations: Yoda translation ")]
    public async Task FunTranslations_Yoda_ShouldReturnOrGracefullyFail()
    {
        try
        {
            // sample from Pikachu 
            const string text = "When several of these POKéMON gather, their electricity could build and cause lightning storms.";

            var res = await _fun.GetYodaTranslationAsync(text);

            res.Should().NotBeNull();
            res!.Text.Should().NotBeNullOrWhiteSpace();

            if (res.isSuccess)
            {
                res.Translation.Should().NotBeNullOrWhiteSpace("Translation is null, Api server error or rate limiter error");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception during Yoda translation: {ex.GetType().Name}: {ex.Message}");
        }
       
    }
}
