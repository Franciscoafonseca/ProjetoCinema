using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddComprasModule(this IServiceCollection services)
    {
        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<ICompraService, CompraService>();
        services.AddScoped<ICompraFactory, CompraFactory>();
        services.AddScoped<ICarrinhoCheckoutService, CarrinhoCheckoutService>();
        services.AddScoped<IAcessoCompraService, AcessoCompraService>();
        services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
        services.AddScoped<ICarrinhoService, CarrinhoService>();
        services.AddScoped<IFinalizacaoCompraService, FinalizacaoCompraService>();
        services.AddScoped<IValidadorFinalizacaoCompra, ValidadorFinalizacaoCompra>();
        services.AddScoped<IGeradorReferenciaCompra, GeradorReferenciaCompra>();
        services.AddScoped<IAcessoUtilizadorFactory, FabricaAcessoUtilizador>();
        services.AddScoped<IPagamentoService, PagamentoSimuladoService>();
        services.AddScoped<IPagamentosPendentesService, PagamentosPendentesService>();
        services.AddScoped<IPagamentoStrategy, PagamentoAprovadoSimuladoStrategy>();
        services.AddScoped<IPagamentoStrategy, PagamentoReferenciaMultibancoStrategy>();
        services.AddScoped<ICarrinhoItemValidator, BilheteSessaoCarrinhoItemValidator>();
        services.AddScoped<ICarrinhoItemValidator, PasseDiarioCarrinhoItemValidator>();
        services.AddScoped<ICarrinhoItemValidator, PasseCompletoCarrinhoItemValidator>();
        services.AddScoped<ICarrinhoItemValidator, AluguerDigitalCarrinhoItemValidator>();

        return services;
    }
}
