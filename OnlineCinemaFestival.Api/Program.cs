using OnlineCinemaFestival.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder
    .Services.AddApiInfrastructure(builder.Environment, builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddAuthorizationPolicies()
    .AddApplicationServices()
    .AddSwaggerWithJwt();

var app = builder.Build();

app.UseApiPipeline();
await app.SeedDevelopmentDataAsync();
app.Run();
