using System.IO;
using Microsoft.AspNetCore.DataProtection;
using OnlineCinemaFestival.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var dataProtectionPath = Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys");

Directory.CreateDirectory(dataProtectionPath);

builder
    .Services.AddDataProtection()
    .SetApplicationName("OnlineCinemaFestival")
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath));

builder
    .Services.AddApiInfrastructure(builder.Environment)
    .AddJwtAuthentication(builder.Configuration)
    .AddAuthorizationPolicies()
    .AddApplicationServices()
    .AddSwaggerWithJwt();

var app = builder.Build();

app.UseApiPipeline();
await app.SeedDevelopmentDataAsync();
app.Run();
