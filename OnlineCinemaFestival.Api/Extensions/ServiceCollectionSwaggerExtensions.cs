using Microsoft.OpenApi;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            const string esquemaAutenticacao = "bearer";

            options.AddSecurityDefinition(
                esquemaAutenticacao,
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insere apenas o token JWT. Nao escrevas Bearer.",
                }
            );

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(esquemaAutenticacao, document)] = [],
            });
        });

        return services;
    }
}
