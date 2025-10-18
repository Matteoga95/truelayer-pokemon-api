using System.Text.Json.Serialization;
namespace Pokemon.Services.Dtos;

internal sealed class SpeciesResponse
{
    [JsonPropertyName("habitat")]
    public HabitatInfo? Habitat { get; init; }

    [JsonPropertyName("is_legendary")]
    public bool IsLegendary { get; set; }

    [JsonPropertyName("flavor_text_entries")]
    public List<FlavorTextEntry> FlavorTextEntries { get; set; } = new();
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

internal sealed class HabitatInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

internal sealed class FlavorTextEntry
{
    [JsonPropertyName("flavor_text")]
    public string FlavorText { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public LanguageInfo Language { get; set; } = new();
}

internal sealed class LanguageInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

