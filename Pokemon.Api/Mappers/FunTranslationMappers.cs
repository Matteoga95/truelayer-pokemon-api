using Pokemon.Api.Dtos;
using Pokemon.Core.Models;

namespace Pokemon.Api.Mappers;

public static class FunTranslationMappers
{
    public static FunTranslationDto ToDto(this FunTranslationDto x) => new()
    {
        Text = x.Text,
        Translation = x.Translation,
        isSuccess = x.isSuccess
    };
}