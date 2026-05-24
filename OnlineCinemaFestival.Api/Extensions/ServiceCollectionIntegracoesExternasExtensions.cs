using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddIntegracoesExternasModule(this IServiceCollection services)
    {
        services.AddScoped<CatalogoTmdbSeedService>();
        services
            .AddHttpClient<ITmdbApiClient, TmdbApiClient>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(8);
            });
        services.AddScoped<ITmdbService, TmdbService>();

        return services;
    }
}
