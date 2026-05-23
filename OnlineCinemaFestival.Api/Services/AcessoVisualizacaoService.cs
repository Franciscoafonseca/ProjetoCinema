using OnlineCinemaFestival.Api.Services.PoliticasAcesso;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services;

public sealed class AcessoVisualizacaoService : IAcessoVisualizacaoService
{
    private readonly IPoliticaAcessoResolver _politicaAcessoResolver;
    private readonly TimeProvider _timeProvider;

    public AcessoVisualizacaoService(
        IPoliticaAcessoResolver politicaAcessoResolver,
        TimeProvider timeProvider
    )
    {
        _politicaAcessoResolver = politicaAcessoResolver;
        _timeProvider = timeProvider;
    }

    public bool PodeVisualizar(ModelAcesso acesso)
    {
        var politica = _politicaAcessoResolver.Resolver(acesso.Tipo);
        var agora = _timeProvider.GetUtcNow().UtcDateTime;

        return politica.TemAcesso(acesso, agora);
    }
}

