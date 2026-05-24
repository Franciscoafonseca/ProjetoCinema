using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddComprasModule(this IServiceCollection services)
    {
        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<ICompraService, CompraService>();
        services.AddScoped<ICompraHistoricoService, CompraHistoricoService>();
        services.AddScoped<ICompraFactory, CompraFactory>();
        services.AddScoped<ICarrinhoCheckoutService, CarrinhoCheckoutService>();
        services.AddScoped<IAcessoCompraService, AcessoCompraService>();
        services.AddScoped<ICompraValidator, CompraValidator>();
        services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
        services.AddScoped<ICarrinhoService, CarrinhoService>();
        services.AddScoped<IFinalizacaoCompraService, FinalizacaoCompraService>();
        services.AddScoped<IValidadorFinalizacaoCompra, ValidadorFinalizacaoCompra>();
        services.AddScoped<IGeradorReferenciaCompra, GeradorReferenciaCompra>();
        services.AddScoped<IAcessoUtilizadorFactory, FabricaAcessoUtilizador>();
        services.AddScoped<IPagamentoService, PagamentoSimuladoService>();
        services.AddScoped<IPrecoStrategy, BilheteIndividualPrecoStrategy>();
        services.AddScoped<IPrecoStrategy, PasseFestivalPrecoStrategy>();
        services.AddScoped<IPagamentoStrategy, PagamentoAprovadoSimuladoStrategy>();
        services.AddScoped<IPagamentoStrategy, PagamentoReferenciaMultibancoStrategy>();
        services.AddScoped<ICarrinhoAcessoStrategy, CarrinhoBilheteSessaoStrategy>();
        services.AddScoped<ICarrinhoAcessoStrategy, CarrinhoPasseDiarioStrategy>();
        services.AddScoped<ICarrinhoAcessoStrategy, CarrinhoPasseCompletoStrategy>();
        services.AddScoped<ICarrinhoAcessoStrategy, CarrinhoAluguerDigitalStrategy>();
        services.AddScoped<ICompraItemValidator, BilheteSessaoCompraItemValidator>();
        services.AddScoped<ICompraItemValidator, AluguerDigitalCompraItemValidator>();

        return services;
    }
}
