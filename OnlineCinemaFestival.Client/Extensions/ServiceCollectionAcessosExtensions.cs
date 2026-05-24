using OnlineCinemaFestival.Client.Services;

namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddClientAcessosModule(this IServiceCollection services)
    {
        services.AddScoped<AcessoService>();
        services.AddScoped<VisualizacaoService>();

        return services;
    }
}
