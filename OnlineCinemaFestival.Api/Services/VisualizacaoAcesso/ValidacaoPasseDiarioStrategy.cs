using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services.VisualizacaoAcesso;

public class ValidacaoPasseDiarioStrategy : IEstrategiaValidacaoAcesso
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;

    public ValidacaoPasseDiarioStrategy(IAcessoUtilizadorRepository acessoUtilizadorRepository)
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
    }

    public TipoAcesso Tipo => TipoAcesso.PasseDiario;

    public Task<AcessoUtilizador?> ValidarFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId,
        DateTime agora
    )
    {
        return Task.FromResult<AcessoUtilizador?>(null);
    }

    public async Task<AcessoUtilizador?> ValidarSessaoAsync(
        int utilizadorId,
        Sessao sessao,
        DateTime agora
    )
    {
        var acesso = await _acessoUtilizadorRepository.ObterAcessoValidoAsync(
            utilizadorId,
            TipoAcesso.PasseDiario,
            agora,
            festivalId: sessao.FestivalId
        );

        return acesso != null
            && sessao.Inicio >= acesso.InicioValidade
            && sessao.Inicio < acesso.FimValidade
            ? acesso
            : null;
    }
}
