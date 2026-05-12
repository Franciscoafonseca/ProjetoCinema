using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ValidacaoAcessoService : IValidacaoAcessoService
{
    private readonly IVisualizacaoRepository _visualizacaoRepository;

    public ValidacaoAcessoService(IVisualizacaoRepository visualizacaoRepository)
    {
        _visualizacaoRepository = visualizacaoRepository;
    }

    public async Task<bool> PodeVisualizarFilmeAsync(int utilizadorId, int filmeId, int? festivalId)
    {
        var agora = DateTime.UtcNow;

        var temAluguer = await _visualizacaoRepository.TemAcessoAtivoParaFilmeAsync(
            utilizadorId,
            filmeId,
            agora
        );

        if (temAluguer)
            return true;

        if (festivalId.HasValue)
        {
            return await _visualizacaoRepository.TemPasseAtivoParaFilmeNoFestivalAsync(
                utilizadorId,
                filmeId,
                festivalId.Value,
                agora
            );
        }

        return false;
    }

    public async Task<bool> PodeVisualizarSessaoAsync(int utilizadorId, Sessao sessao)
    {
        var agora = DateTime.UtcNow;

        return await _visualizacaoRepository.TemAcessoAtivoParaSessaoAsync(
            utilizadorId,
            sessao,
            agora
        );
    }
}
