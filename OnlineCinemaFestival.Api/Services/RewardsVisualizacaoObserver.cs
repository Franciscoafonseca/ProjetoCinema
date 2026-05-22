using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsVisualizacaoObserver : IVisualizacaoObserver
{
    private readonly IRewardsRepository _rewardsRepository;
    private readonly IRewardTransacaoRepository _transacaoRepository;

    public RewardsVisualizacaoObserver(
        IRewardsRepository rewardsRepository,
        IRewardTransacaoRepository transacaoRepository
    )
    {
        _rewardsRepository = rewardsRepository;
        _transacaoRepository = transacaoRepository;
    }

    public async Task NotificarAsync(Visualizacao visualizacao)
    {
        await _rewardsRepository.AddOrUpdatePointsAsync(visualizacao.UtilizadorId, 1);
        await _transacaoRepository.AddAsync(
            new RewardTransacao
            {
                UtilizadorId = visualizacao.UtilizadorId,
                Pontos = 1,
                Motivo = "Visualizacao",
            }
        );
        await _transacaoRepository.SaveChangesAsync();
    }
}
