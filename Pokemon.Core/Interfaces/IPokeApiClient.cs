using Pokemon.Core.Models;
namespace Pokemon.Core.Interfaces;


public interface IPokeApiClient
{
    Task<PokemonInfo?> GetPokemonInfoAsync(string name);
}