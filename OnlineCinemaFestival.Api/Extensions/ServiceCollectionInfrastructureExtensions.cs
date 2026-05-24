using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Data;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiInfrastructure(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration
    )
    {
        services.AddControllers();
        services.AddMemoryCache();
        services.AddApiDataProtection(environment);
        services.AddApiDatabase(configuration);
        services.AddApiCors(configuration);
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();

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
                "ConnectionStrings:DefaultConnection nao configurada no appsettings.json."
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
                "Cors:AllowedOrigins nao configurado no appsettings.json."
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
