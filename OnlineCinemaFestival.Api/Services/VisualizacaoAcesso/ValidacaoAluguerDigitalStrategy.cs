using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services.VisualizacaoAcesso;

public class ValidacaoAluguerDigitalStrategy : IEstrategiaValidacaoAcesso
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;

    public ValidacaoAluguerDigitalStrategy(IAcessoUtilizadorRepository acessoUtilizadorRepository)
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
    }

    public TipoAcesso Tipo => TipoAcesso.AluguerDigital;

    public async Task<AcessoUtilizador?> ValidarFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId,
        DateTime agora
    )
    {
        return await _acessoUtilizadorRepository.ObterAcessoValidoAsync(
            utilizadorId,
            TipoAcesso.AluguerDigital,
            agora,
            filmeId: filme.Id
        );
    }

    public Task<AcessoUtilizador?> ValidarSessaoAsync(
        int utilizadorId,
        Sessao sessao,
        DateTime agora
    )
    {
        return Task.FromResult<AcessoUtilizador?>(null);
    }
}
