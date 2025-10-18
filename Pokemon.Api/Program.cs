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

// Swagger dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // 👇 automatically collapse all response sections
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

app.MapControllers();
app.Run();