using Pokemon.Core.Interfaces;
using Pokemon.Services.ApiClients;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var pokeApiUrl = builder.Configuration["ExternalApis:PokeApi"];


builder.Services.AddHttpClient<IPokeApiClient, PokeApiClient>(c =>
{
    c.BaseAddress = new Uri(pokeApiUrl ?? throw new InvalidOperationException("PokeApi URL not configured."));
});

builder.Services.AddHttpClient<IFunTranslationApiClient, FunTranslationApiClient>(c => { });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();