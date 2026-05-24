using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsAvaliacaoObserver : IAvaliacaoObserver
{
    private readonly IRewardsPontuacaoService _rewardsPontuacaoService;

    public RewardsAvaliacaoObserver(IRewardsPontuacaoService rewardsPontuacaoService)
    {
        _rewardsPontuacaoService = rewardsPontuacaoService;
    }

    public async Task NotificarAsync(Avaliacao avaliacao)
    {
        await _rewardsPontuacaoService.AtribuirSeAindaNaoAtribuidoAsync(
            avaliacao.UsuarioId,
            2,
            "Avaliacao",
            $"avaliacao:{avaliacao.FilmeId}"
        );
    }
}
