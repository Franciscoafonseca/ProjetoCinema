using Microsoft.AspNetCore.Components.Authorization;
using OnlineCinemaFestival.Client.Services;

namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientAuthentication(this IServiceCollection services)
    {
        services.AddScoped<ArmazenamentoToken>();
        services.AddScoped<ManipuladorTokenHttp>();
        services.AddScoped<EstadoAutenticacaoCustomizado>();
        services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<EstadoAutenticacaoCustomizado>()
        );
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        return services;
    }
}
