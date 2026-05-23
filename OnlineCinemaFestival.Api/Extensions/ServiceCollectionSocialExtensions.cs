using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddSocialModule(this IServiceCollection services)
    {
        services.AddScoped<IUtilizadorAtualService, UtilizadorAtualService>();
        services.AddScoped<IUtilizadorRepository, UtilizadorRepository>();
        services.AddScoped<IPerfilFotoUploadService, PerfilFotoUploadService>();
        services.AddScoped<IPerfilUtilizadorService, PerfilUtilizadorService>();
        services.AddScoped<IListaPessoalRepository, ListaPessoalRepository>();
        services.AddScoped<IListaPessoalService, ListaPessoalService>();
        services.AddScoped<IComentarioRepository, ComentarioRepository>();
        services.AddScoped<IComentarioService, ComentarioService>();
        services.AddScoped<IComunidadeRepository, ComunidadeRepository>();
        services.AddScoped<IComunidadeService, ComunidadeService>();

        return services;
    }
}
