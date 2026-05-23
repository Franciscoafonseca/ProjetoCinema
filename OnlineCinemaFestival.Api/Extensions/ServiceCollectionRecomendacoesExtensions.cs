using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddRecomendacoesModule(this IServiceCollection services)
    {
        services.AddScoped<IRewardsRepository, RewardsRepository>();
        services.AddScoped<IRewardTransacaoRepository, RewardTransacaoRepository>();
        services.AddScoped<IRewardsQueryService, RewardsQueryService>();
        services.AddScoped<ICompraObserver, AcessoObserver>();
        services.AddScoped<ICompraObserver, RewardsObserver>();
        services.AddScoped<IVisualizacaoObserver, RewardsVisualizacaoObserver>();
        services.AddScoped<IAvaliacaoObserver, RewardsAvaliacaoObserver>();

        return services;
    }
}
