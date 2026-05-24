using OnlineCinemaFestival.Client.Services;

namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddClientComprasModule(this IServiceCollection services)
    {
        services.AddScoped<ICarrinhoService, CarrinhoService>();
        services.AddScoped<IFinalizacaoCompraService, FinalizacaoCompraService>();
        services.AddScoped<CompraService>();
        services.AddScoped<CartService>();
        services.AddScoped<CarrinhoApiService>();
        services.AddScoped<ComprasHistoricoService>();

        return services;
    }
}
