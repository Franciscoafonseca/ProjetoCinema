using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsVisualizacaoObserver : IVisualizacaoObserver
{
    private readonly IRewardsPontuacaoService _rewardsPontuacaoService;

    public RewardsVisualizacaoObserver(IRewardsPontuacaoService rewardsPontuacaoService)
    {
        _rewardsPontuacaoService = rewardsPontuacaoService;
    }

    public async Task NotificarAsync(Visualizacao visualizacao)
    {
        var alvo = visualizacao.SessaoId.HasValue
            ? $"sessao:{visualizacao.SessaoId.Value}"
            : $"filme:{visualizacao.FilmeId}";

        await _rewardsPontuacaoService.AtribuirSeAindaNaoAtribuidoAsync(
            visualizacao.UtilizadorId,
            1,
            "Visualizacao",
            $"visualizacao:{alvo}"
        );
    }
}
