using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services.VisualizacaoAcesso;

public class ValidacaoBilheteSessaoStrategy : IEstrategiaValidacaoAcesso
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;

    public ValidacaoBilheteSessaoStrategy(IAcessoUtilizadorRepository acessoUtilizadorRepository)
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
    }

    public TipoAcesso Tipo => TipoAcesso.BilheteSessao;

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
        return await _acessoUtilizadorRepository.ObterAcessoValidoAsync(
            utilizadorId,
            TipoAcesso.BilheteSessao,
            agora,
            sessaoId: sessao.Id
        );
    }
}
