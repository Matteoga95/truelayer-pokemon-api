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
        var info = await pokeApi.GetPokemonInfoAsync(name);
        if (info is null) return NotFound(new { message = $"Pokémon '{name}' not found." });
        return Ok(info);
    }
    
    
    // GET /pokemon/translated/{name}
    [HttpGet("translated/{name}")]
    public async Task<ActionResult<PokemonInfo>> GetTranslated(string name)
    {
        var pokemonInfo = await pokeApi.GetPokemonInfoAsync(name);
        if (pokemonInfo is null) return NotFound(new { message = $"Pokémon '{name}' not found." });
         
        var useYoda = pokemonInfo.IsLegendary 
                      || string.Equals(pokemonInfo.Habitat?.Trim(), "cave", StringComparison.InvariantCultureIgnoreCase);

        var translation = useYoda
            ? await funTranslate.GetYodaTranslationAsync(pokemonInfo.Description)
            : await funTranslate.GetShakespeareTranslationAsync(pokemonInfo.Description);

        if (translation?.isSuccess == true)
            pokemonInfo.Description = translation.Translation;
        return Ok(pokemonInfo);
    }
}