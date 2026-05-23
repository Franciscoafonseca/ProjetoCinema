using OnlineCinemaFestival.Client.Services;

namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddClientCatalogoModule(this IServiceCollection services)
    {
        services.AddScoped<FilmeService>();
        services.AddScoped<IConsultaFilmeService>(sp => sp.GetRequiredService<FilmeService>());
        services.AddScoped<IImportacaoFilmeService>(sp => sp.GetRequiredService<FilmeService>());
        services.AddScoped<FestivalService>();
        services.AddScoped<SessaoService>();
        services.AddScoped<AvaliacaoService>();
        services.AddScoped<ListaService>();

        return services;
    }
}
