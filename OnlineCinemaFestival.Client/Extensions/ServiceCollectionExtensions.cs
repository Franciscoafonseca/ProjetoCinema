namespace OnlineCinemaFestival.Client.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientApplicationServices(this IServiceCollection services)
    {
        return services
            .AddClientCatalogoModule()
            .AddClientComprasModule()
            .AddClientAcessosModule()
            .AddClientSocialModule()
            .AddClientRecomendacoesModule()
            .AddClientPremiosModule()
            .AddClientIntegracoesExternasModule();
    }
}
