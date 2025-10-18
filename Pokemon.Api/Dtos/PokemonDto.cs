﻿namespace Pokemon.Api.Dtos;

public sealed class PokemonDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Habitat { get; init; } = string.Empty;
    public bool IsLegendary { get; init; }
}
