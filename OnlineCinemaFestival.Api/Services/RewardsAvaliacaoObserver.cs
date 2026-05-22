using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsAvaliacaoObserver : IAvaliacaoObserver
{
    private readonly IRewardsRepository _rewardsRepository;
    private readonly IRewardTransacaoRepository _transacaoRepository;

    public RewardsAvaliacaoObserver(
        IRewardsRepository rewardsRepository,
        IRewardTransacaoRepository transacaoRepository
    )
    {
        _rewardsRepository = rewardsRepository;
        _transacaoRepository = transacaoRepository;
    }

    public async Task NotificarAsync(Avaliacao avaliacao)
    {
        await _rewardsRepository.AddOrUpdatePointsAsync(avaliacao.UsuarioId, 2);
        await _transacaoRepository.AddAsync(
            new RewardTransacao
            {
                UtilizadorId = avaliacao.UsuarioId,
                Pontos = 2,
                Motivo = "Avaliacao",
            }
        );
        await _transacaoRepository.SaveChangesAsync();
    }
}
