using OnlineCinemaFestival.Client.Services;

namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddClientComprasModule(this IServiceCollection services)
    {
        services.AddScoped<ICarrinhoService, CarrinhoService>();
        services.AddScoped<IFinalizacaoCompraService, FinalizacaoCompraService>();
        services.AddScoped<CompraService>();

        return services;
    }
}
