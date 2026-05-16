using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services.VisualizacaoAcesso;

namespace OnlineCinemaFestival.Api.Services;

public class ValidacaoAcessoService : IValidacaoAcessoService
{
    private readonly IVisualizacaoRepository _visualizacaoRepository;
    private readonly IReadOnlyList<IEstrategiaValidacaoAcesso> _estrategias;

    public ValidacaoAcessoService(
        IVisualizacaoRepository visualizacaoRepository,
        IEnumerable<IEstrategiaValidacaoAcesso> estrategias
    )
    {
        _visualizacaoRepository = visualizacaoRepository;
        _estrategias = estrategias.OrderBy(e => OrdemPreferencia(e.Tipo)).ToList();
    }

    public async Task<AcessoUtilizador?> ObterAcessoValidoParaFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId
    )
    {
        var agora = DateTime.UtcNow;

        foreach (var estrategia in _estrategias)
        {
            var acesso = await estrategia.ValidarFilmeAsync(utilizadorId, filme, festivalId, agora);

            if (acesso != null)
                return acesso;
        }

        return null;
    }

    public async Task<AcessoUtilizador?> ObterAcessoValidoParaSessaoAsync(
        int utilizadorId,
        Sessao sessao
    )
    {
        var agora = DateTime.UtcNow;

        foreach (var estrategia in _estrategias)
        {
            var acesso = await estrategia.ValidarSessaoAsync(utilizadorId, sessao, agora);

            if (acesso != null)
                return acesso;
        }

        return null;
    }

    public async Task<bool> PodeVisualizarFilmeAsync(int utilizadorId, int filmeId, int? festivalId)
    {
        var filme = await _visualizacaoRepository.GetFilmeByIdAsync(filmeId);

        if (filme == null)
            return false;

        return await ObterAcessoValidoParaFilmeAsync(utilizadorId, filme, festivalId) != null;
    }

    public async Task<bool> PodeVisualizarSessaoAsync(int utilizadorId, Sessao sessao)
    {
        return await ObterAcessoValidoParaSessaoAsync(utilizadorId, sessao) != null;
    }

    private static int OrdemPreferencia(TipoAcesso tipo)
    {
        return tipo switch
        {
            TipoAcesso.BilheteSessao => 0,
            TipoAcesso.AluguerDigital => 1,
            TipoAcesso.PasseDiario => 2,
            TipoAcesso.PasseCompleto => 3,
            _ => 99,
        };
    }
}
