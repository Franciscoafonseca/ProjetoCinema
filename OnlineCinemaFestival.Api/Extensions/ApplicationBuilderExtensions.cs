using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Hubs;
using OnlineCinemaFestival.Api.Middleware;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(NomesCors.BlazorClient);

        if (Directory.Exists(app.Environment.WebRootPath))
            app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<SessaoChatHub>("/hubs/sessoes-chat");

        return app;
    }

    public static async Task SeedDevelopmentDataAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return;

        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHashingStrategy =
            scope.ServiceProvider.GetRequiredService<IPasswordHashingStrategy>();

        await DbSeeder.SeedAsync(db, passwordHashingStrategy, app.Configuration);

        await scope.ServiceProvider.GetRequiredService<IPublicacaoPremiosService>()
            .PublicarResultadosPendentesAsync();

        try
        {
            await scope.ServiceProvider.GetRequiredService<CatalogoTmdbSeedService>()
                .GarantirCatalogoPopularAsync();
        }
        catch (Exception ex) when (
            ex is HttpRequestException
            || ex is TaskCanceledException
            || ex is InvalidOperationException
        )
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger("CatalogoTmdbSeed");
            logger.LogWarning(ex, "Seed TMDB ignorado: TMDB indisponivel ou nao configurado.");
        }
    }
}
