namespace OnlineCinemaFestival.Api.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        return services
            .AddAutenticacaoModule()
            .AddCatalogoModule()
            .AddAcessosModule()
            .AddComprasModule()
            .AddSocialModule()
            .AddRecomendacoesModule()
            .AddPremiosModule()
            .AddIntegracoesExternasModule()
            .AddSignalRModule();
    }
}
