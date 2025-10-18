using System.Net.Http;
using System.Text.Json;
using Pokemon.Core.Interfaces;
using Pokemon.Core.Models;
using Pokemon.Services.Dtos;

namespace Pokemon.Services.ApiClients;
public class PokeApiClient(HttpClient http, IConfiguration config) : IPokeApiClient
{
    public async Task<PokemonInfo?> GetPokemonInfoAsync(string name)
    {
        var baseUrl = config["ExternalApis:PokeApi:BaseUrl"]!;
        var pathTemplate = config["ExternalApis:PokeApi:Endpoints:Species"] ?? "pokemon-species/{name}";
        var url = new Uri(new Uri(baseUrl), pathTemplate.Replace("{name}", name.ToLowerInvariant()));

        using var response = await http.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<SpeciesResponse>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (data is null) return null;

        var desc = data.FlavorTextEntries?
            .FirstOrDefault(e => e.Language?.Name.Equals("en", StringComparison.OrdinalIgnoreCase) == true)
            ?.FlavorText ?? string.Empty;

        desc = desc.Replace("\n", " ").Replace("\f", " ").Trim();

        return new PokemonInfo
        {
            Name = name,
            Description = desc,
            Habitat = data.Habitat?.Name ?? string.Empty,
            IsLegendary = data.IsLegendary
        };
    }
}
    
