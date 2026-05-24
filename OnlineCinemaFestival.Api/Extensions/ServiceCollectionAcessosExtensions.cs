using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Services.AcessosFolder;
using OnlineCinemaFestival.Api.Services.PoliticasAcesso;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddAcessosModule(this IServiceCollection services)
    {
        services.AddScoped<IAcessoRepository, AcessoRepository>();
        services.AddScoped<IAcessoService, AcessoService>();
        services.AddScoped<IAcessoUtilizadorRepository, AcessoUtilizadorRepository>();
        services.AddScoped<IAcessoUtilizadorService, AcessoUtilizadorService>();
        services.AddScoped<IAcessoVisualizacaoService, AcessoVisualizacaoService>();
        services.AddScoped<IValidacaoAcessoService, ValidacaoAcessoService>();
        services.AddScoped<IVisualizacaoRepository, VisualizacaoRepository>();
        services.AddScoped<IVisualizacaoService, VisualizacaoService>();
        services.AddScoped<IAcessoAutomaticoFactory, AcessoAutomaticoFactory>();
        services.AddScoped<IAcessoAutomaticoService, AcessoAutomaticoService>();
        services.AddScoped<IAcessoFactory, AcessoFactory>();
        services.AddScoped<IValidacaoAcessoStrategyFactory, ValidacaoAcessoStrategyFactory>();

        services.AddScoped<
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoBilheteSessaoStrategy
        >();
        services.AddScoped<
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoPasseDiarioStrategy
        >();
        services.AddScoped<
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoPasseCompletoStrategy
        >();
        services.AddScoped<
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoAluguerDigitalStrategy
        >();

        services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoBilheteSessao>();
        services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoPasseDiario>();
        services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoPasseCompleto>();
        services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoAluguerDigital>();
        services.AddScoped<IEstrategiaValidacaoAcesso, BilheteSessaoValidacaoStrategy>();
        services.AddScoped<IEstrategiaValidacaoAcesso, EstrategiaValidacaoPasseDiario>();
        services.AddScoped<IEstrategiaValidacaoAcesso, ValidacaoPasseCompletoStrategy>();
        services.AddScoped<IEstrategiaValidacaoAcesso, AluguerDigitalValidacaoStrategy>();
        services.AddScoped<IPoliticaAcesso, PoliticaBilheteSessao>();
        services.AddScoped<IPoliticaAcesso, PoliticaPasseDiario>();
        services.AddScoped<IPoliticaAcesso, PoliticaPasseCompleto>();
        services.AddScoped<IPoliticaAcesso, PoliticaAluguerDigital>();
        services.AddScoped<IPoliticaAcessoResolver, PoliticaAcessoResolver>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
