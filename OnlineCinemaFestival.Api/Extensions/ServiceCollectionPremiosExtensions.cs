using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddPremiosModule(this IServiceCollection services)
    {
        services.AddScoped<IPremioFestivalRepository, PremioFestivalRepository>();
        services.AddScoped<IPremioFestivalService, PremioFestivalService>();
        services.AddScoped<IPublicacaoPremiosService, PublicacaoPremiosService>();
        services.AddHostedService<PublicacaoPremiosBackgroundService>();

        return services;
    }
}
