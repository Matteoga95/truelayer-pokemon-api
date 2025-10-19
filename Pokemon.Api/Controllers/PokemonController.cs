using Microsoft.AspNetCore.Mvc;
using Pokemon.Core.Interfaces;
using Pokemon.Core.Models;
namespace Pokemon.Api.Controllers;

[ApiController]
[Route("pokemon")]
public class PokemonController(IPokeApiClient pokeApi, IFunTranslationApiClient funTranslate) : ControllerBase
{
    // GET /pokemon/{name}
    [HttpGet("{name}")]
    public async Task<ActionResult<PokemonInfo>> Get(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return this.InvalidPokemonName();
        try
        {
            var info = await pokeApi.GetPokemonInfoAsync(name);
            if (info is null) return this.PokemonNotFound(name);
            return Ok(info);
        }
        catch (Exception ex)
        {
            return this.PokeApiError(ex);
        }
    }
    
    
    // GET /pokemon/translated/{name}
    [HttpGet("translated/{name}")]
    public async Task<ActionResult<PokemonInfo>> GetTranslated(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return this.InvalidPokemonName();
        try
        {
            var pokemonInfo = await pokeApi.GetPokemonInfoAsync(name);
            if (pokemonInfo is null) return this.PokemonNotFound(name);
         
            var useYoda = pokemonInfo.IsLegendary 
                          || string.Equals(pokemonInfo.Habitat?.Trim(), "cave", StringComparison.InvariantCultureIgnoreCase);

            var translation = useYoda
                ? await funTranslate.GetYodaTranslationAsync(pokemonInfo.Description)
                : await funTranslate.GetShakespeareTranslationAsync(pokemonInfo.Description);

            if (translation?.isSuccess == true)
                pokemonInfo.Description = translation.Translation;
            return Ok(pokemonInfo);
        }
        catch (Exception ex)
        {
            return this.FunTranslationApiError(ex);
        }
    }
}


public static class ResultsExtensions
{
    public static ActionResult InvalidPokemonName(this ControllerBase c)
        => c.BadRequest(new ProblemDetails
        {
            Title = "Invalid Pokémon name",
            Detail = "You must provide a non-empty Pokémon name.",
            Status = StatusCodes.Status400BadRequest,
            Instance = c.HttpContext?.Request?.Path.Value
        });
    
    public static ActionResult PokemonNotFound(this ControllerBase c, string name)
        => c.NotFound(new ProblemDetails
        {
            Title = "Pokémon not found",
            Detail = $"Pokémon '{name}' was not found in PokéAPI.",
            Status = StatusCodes.Status404NotFound,
            Instance = c.HttpContext?.Request?.Path.Value
        });
    
    public static ActionResult PokeApiError(this ControllerBase c,Exception ex)
        => c.BadRequest(new ProblemDetails
        {
            Title = "Error (PokéAPI)",
            Detail = $"Failed to retrieve data from PokéAPI. Error message: {ex.Message}",
            Status = StatusCodes.Status502BadGateway,
            Instance = c.HttpContext?.Request?.Path.Value
        });
    
    public static ActionResult FunTranslationApiError(this ControllerBase c,Exception ex)
        => c.BadRequest(new ProblemDetails
        {
            Title = "Error (FunTranslations)",
            Detail = $"Unexpected error while translating the description. Error message: {ex.Message}",
            Status = StatusCodes.Status502BadGateway,
            Instance = c.HttpContext?.Request?.Path.Value
        });


}