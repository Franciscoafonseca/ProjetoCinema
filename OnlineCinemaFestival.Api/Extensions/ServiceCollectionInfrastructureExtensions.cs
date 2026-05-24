using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Infrastructure.Data;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiInfrastructure(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration
    )
    {
        services.AddConfigurationOptions(configuration);
        services.AddControllers();
        services.AddMemoryCache();
        services.AddApiDataProtection(environment);
        services.AddApiDatabase(configuration);
        services.AddApiCors(configuration);
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();

        return services;
    }

    private static IServiceCollection AddConfigurationOptions(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Key)
                    && !string.IsNullOrWhiteSpace(options.Issuer)
                    && !string.IsNullOrWhiteSpace(options.Audience)
                    && options.ExpirationMinutes > 0,
                "Configuracao Jwt invalida."
            )
            .ValidateOnStart();

        services
            .AddOptions<CorsOptions>()
            .Bind(configuration.GetSection(CorsOptions.SectionName))
            .Validate(
                options => options.AllowedOrigins.Length > 0,
                "Configuracao Cors:AllowedOrigins invalida."
            )
            .ValidateOnStart();

        services
            .AddOptions<TmdbOptions>()
            .Bind(configuration.GetSection(TmdbOptions.SectionName))
            .Validate(
                options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
                "Configuracao Tmdb:BaseUrl invalida."
            )
            .ValidateOnStart();

        services
            .AddOptions<YouTubeOptions>()
            .Bind(configuration.GetSection(YouTubeOptions.SectionName))
            .Validate(
                options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
                "Configuracao YouTube:BaseUrl invalida."
            )
            .ValidateOnStart();

        services
            .AddOptions<PagamentoOptions>()
            .Bind(configuration.GetSection(PagamentoOptions.SectionName))
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Multibanco.Entidade)
                    && options.Multibanco.ExpiracaoHoras > 0,
                "Configuracao Pagamentos:Multibanco invalida."
            )
            .ValidateOnStart();

        services
            .AddOptions<AcessosOptions>()
            .Bind(configuration.GetSection(AcessosOptions.SectionName))
            .Validate(
                options =>
                    options.QuantidadeMaximaCarrinho > 0
                    && options.DuracaoAluguerDigitalHoras > 0
                    && options.DescontoFestival is >= 0 and < 1
                    && options.ValidadePasseCompletoDias > 0
                    && options.Precos.Values.All(preco => preco > 0),
                "Configuracao Acessos invalida."
            )
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddApiDataProtection(
        this IServiceCollection services,
        IWebHostEnvironment environment
    )
    {
        var dataProtectionKeysPath = Path.Combine(
            environment.ContentRootPath,
            "DataProtectionKeys"
        );

        Directory.CreateDirectory(dataProtectionKeysPath);

        var dataProtectionBuilder = services
            .AddDataProtection()
            .SetApplicationName("OnlineCinemaFestival")
            .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));

        if (environment.IsDevelopment() && OperatingSystem.IsWindows())
            dataProtectionBuilder.ProtectKeysWithDpapi();

        return services;
    }

    private static IServiceCollection AddApiDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection nao configurada na configuracao."
            );

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        return services;
    }

    private static IServiceCollection AddApiCors(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var corsOrigins =
            configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? Array.Empty<string>();

        if (corsOrigins.Length == 0)
            throw new InvalidOperationException(
                "Cors:AllowedOrigins nao configurado na configuracao."
            );

        services.AddCors(options =>
        {
            options.AddPolicy(
                NomesCors.BlazorClient,
                policy =>
                {
                    policy
                        .WithOrigins(corsOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
            );
        });

        return services;
    }
}
