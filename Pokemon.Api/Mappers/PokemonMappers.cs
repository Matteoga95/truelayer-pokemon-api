using Pokemon.Api.Dtos;
using Pokemon.Core.Models;

namespace Pokemon.Api.Mappers;

public static class PokemonMappers
{
    public static PokemonDto ToDto(this PokemonInfo x) => new()
    {
        Name = x.Name,
        Description = x.Description,
        Habitat = x.Habitat,
        IsLegendary = x.IsLegendary
    };
}