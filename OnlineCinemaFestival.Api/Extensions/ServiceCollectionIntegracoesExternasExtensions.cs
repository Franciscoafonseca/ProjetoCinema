using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddIntegracoesExternasModule(this IServiceCollection services)
    {
        services.AddScoped<ICinemaFacade, CinemaFacade>();
        services.AddScoped<CatalogoTmdbSeedService>();
        services.AddHttpClient<ITmdbApiClient, TmdbApiClient>();
        services.AddScoped<ITmdbService, TmdbService>();

        return services;
    }
}
