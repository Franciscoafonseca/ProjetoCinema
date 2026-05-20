using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services.VisualizacaoAcesso;

public class ValidacaoPasseCompletoStrategy : IEstrategiaValidacaoAcesso
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;

    public ValidacaoPasseCompletoStrategy(IAcessoUtilizadorRepository acessoUtilizadorRepository)
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
    }

    public TipoAcesso Tipo => TipoAcesso.PasseCompleto;

    public async Task<AcessoUtilizador?> ValidarFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId,
        DateTime agora
    )
    {
        return await _acessoUtilizadorRepository.ObterPasseCompletoValidoParaFilmeAsync(
            utilizadorId,
            filme.Id,
            festivalId,
            agora
        );
    }

    public async Task<AcessoUtilizador?> ValidarSessaoAsync(
        int utilizadorId,
        Sessao sessao,
        DateTime agora
    )
    {
        return await _acessoUtilizadorRepository.ObterAcessoValidoAsync(
            utilizadorId,
            TipoAcesso.PasseCompleto,
            agora,
            festivalId: sessao.FestivalId
        );
    }
}
