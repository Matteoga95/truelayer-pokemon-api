using Pokemon.Core.Models;
namespace Pokemon.Core.Interfaces;

public interface IFunTranslationApiClient
{
    Task<FunTranslation?> GetYodaTranslationAsync(string text);
    Task<FunTranslation?> GetShakespeareTranslationAsync(string text);
}
