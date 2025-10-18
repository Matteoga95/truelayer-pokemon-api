using System.Net;
using System.Text;
using System.Text.Json;
using Pokemon.Core.Interfaces;
using Pokemon.Core.Models;
using Pokemon.Services.Dtos;

namespace Pokemon.Services.ApiClients;

public class FunTranslationApiClient(HttpClient http, IConfiguration config) : IFunTranslationApiClient
{
    private readonly string _baseUrl = config["ExternalApis:FunTranslation:BaseUrl"] 
                                       ?? throw new InvalidOperationException("Missing ExternalApis:FunTranslation:BaseUrl in configuration.");

    public Task<FunTranslation?> GetYodaTranslationAsync(string text)
    {
        var pathTemplate = config["ExternalApis:FunTranslation:Endpoints:Yoda"];
        var url = $"{_baseUrl}{pathTemplate}";
        return TranslateAsync(url, text);
    }

    public Task<FunTranslation?> GetShakespeareTranslationAsync(string text)
    {
        var pathTemplate = config["ExternalApis:FunTranslation:Endpoints:shakespeare"];
        var url = $"{_baseUrl}{pathTemplate}";
        return TranslateAsync(url, text);
    }
    
     private async Task<FunTranslation?> TranslateAsync(string url, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new FunTranslation { Text = string.Empty, Translation = string.Empty, isSuccess = false };
        
        // POST form: text=...
        using var content = new StringContent($"text={Uri.EscapeDataString(text)}", Encoding.UTF8, "application/x-www-form-urlencoded");
        using var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        
        using var resp = await http.SendAsync(req);

        // Handle common non-success cases gracefully
        if (resp.StatusCode == HttpStatusCode.TooManyRequests)  // 429 rate limit (public plan)
        {
            return new FunTranslation
            {
                Text = text,
                Translation = "Rate limit exceeded. Try later.",
                isSuccess = false
            };
        }

        if (!resp.IsSuccessStatusCode)
        {
            // Other errors -> surface a failure object; you could also throw a typed exception if you prefer
            var body = await SafeReadBodyRespAsync(resp);
            return new FunTranslation
            {
                Text = text,
                Translation = $"FunTranslations error {(int)resp.StatusCode}: {body}",
                isSuccess = false
            };
        }

        await using var stream = await resp.Content.ReadAsStreamAsync();
        var payload = await JsonSerializer.DeserializeAsync<TranslateResponse>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (payload?.Content?.Translated is null)
        {
            return new FunTranslation { Text = text, Translation = string.Empty, isSuccess = false };
        }

        return new FunTranslation
        {
            Text = payload.Content.Text ?? text,
            Translation = payload.Content.Translated,
            isSuccess = payload.Success?.Total == 1
        };
    }
    
    private static async Task<string> SafeReadBodyRespAsync(HttpResponseMessage resp, int max = 200)
    {
        try
        {
            var t = await resp.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(t)) return "<empty>";
            return t.Length <= max ? t : t[..max] + "…";
        }
        catch { return "<unreadable>"; }
    }
}