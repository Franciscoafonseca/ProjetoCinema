using OnlineCinemaFestival.Client.Services;

namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddClientRecomendacoesModule(this IServiceCollection services)
    {
        services.AddScoped<RewardsService>();
        services.AddScoped<RecomendacaoService>();

        return services;
    }
}
