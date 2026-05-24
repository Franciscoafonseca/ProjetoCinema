using OnlineCinemaFestival.Client.Services;

namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddClientSocialModule(this IServiceCollection services)
    {
        services.AddScoped<PerfilEstadoService>();
        services.AddScoped<PerfilService>();
        services.AddScoped<ComunidadeService>();
        services.AddScoped<ComentarioService>();

        return services;
    }
}
