using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsVotoPremioObserver : IVotoPremioObserver
{
    private readonly IRewardsPontuacaoService _rewardsPontuacaoService;

    public RewardsVotoPremioObserver(IRewardsPontuacaoService rewardsPontuacaoService)
    {
        _rewardsPontuacaoService = rewardsPontuacaoService;
    }

    public Task NotificarAsync(VotoPremioFestival voto)
    {
        return _rewardsPontuacaoService.AtribuirSeAindaNaoAtribuidoAsync(
            voto.UtilizadorId,
            2,
            "Voto em premio",
            $"voto-premio:{voto.PremioFestivalId}"
        );
    }
}
