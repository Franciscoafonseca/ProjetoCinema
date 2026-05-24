using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Services.Catalogo;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddCatalogoModule(this IServiceCollection services)
    {
        services.AddScoped<IFilmeRepository, FilmeRepository>();
        services.AddScoped<IFilmeService, FilmeService>();
        services.AddScoped<IFestivalRepository, FestivalRepository>();
        services.AddScoped<IFestivalService, FestivalService>();
        services.AddScoped<IFestivalFilmeRepository, FestivalFilmeRepository>();
        services.AddScoped<IFestivalFilmeService, FestivalFilmeService>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ISessaoRepository, SessaoRepository>();
        services.AddScoped<ISessaoService, SessaoService>();
        services.AddScoped<ICatalogoService, CatalogoService>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorTituloStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorPopularidadeStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorClassificacaoStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorDataLancamentoStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorVisualizacoesStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorFestivalStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategyFactory, CatalogoOrdenacaoStrategyFactory>();

        return services;
    }
}
