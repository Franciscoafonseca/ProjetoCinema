using OnlineCinemaFestival.Client.Services;

namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddClientPremiosModule(this IServiceCollection services)
    {
        services.AddScoped<PremioFestivalService>();

        return services;
    }
}
