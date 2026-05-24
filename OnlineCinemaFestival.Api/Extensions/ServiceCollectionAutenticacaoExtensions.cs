using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddAutenticacaoModule(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHashingStrategy, PasswordHashingStrategy>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();
        services.AddScoped<IAutenticacaoExternaService, AutenticacaoExternaService>();

        return services;
    }
}
