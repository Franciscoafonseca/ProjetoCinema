using MudBlazor.Services;
using OnlineCinemaFestival.Client.Services;

namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var apiBaseUrl =
            configuration["Api:BaseUrl"]
            ?? throw new InvalidOperationException("Api:BaseUrl nao configurado.");

        services
            .AddHttpClient(
                "Api",
                cliente =>
                {
                    cliente.BaseAddress = new Uri(apiBaseUrl);
                }
            )
            .AddHttpMessageHandler<ManipuladorTokenHttp>();

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));
        services.AddScoped<ImagemUrlService>();
        services.AddMudServices();

        return services;
    }
}
