using System.Text.Json.Serialization;

namespace Pokemon.Services.Dtos;

internal sealed class TranslateResponse
{
    [JsonPropertyName("success")]
    public Success Success { get; set; }
    [JsonPropertyName("contents")]
    public Content Content { get; set; }
}
internal sealed class Success
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
    
}
internal sealed class Content
{
    [JsonPropertyName("translated")]
    public string? Translated { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("translation")]
    public string? Translation { get; set; }
    
}