using OnlineCinemaFestival.Client.Services;

namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddClientIntegracoesExternasModule(
        this IServiceCollection services
    )
    {
        services.AddScoped<TmdbService>();

        return services;
    }
}
