using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddSignalRModule(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddScoped<IMensagemChatSessaoRepository, MensagemChatSessaoRepository>();
        services.AddScoped<IChatSessaoService, ChatSessaoService>();

        return services;
    }
}
