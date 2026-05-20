using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsQueryService : IRewardsQueryService
{
    private readonly IRewardsRepository _rewardsRepository;
    private readonly IRewardTransacaoRepository _rewardTransacaoRepository;

    public RewardsQueryService(
        IRewardsRepository rewardsRepository,
        IRewardTransacaoRepository rewardTransacaoRepository
    )
    {
        _rewardsRepository = rewardsRepository;
        _rewardTransacaoRepository = rewardTransacaoRepository;
    }

    public int ObterSaldo(int utilizadorId)
    {
        return _rewardsRepository.ObterSaldo(utilizadorId);
    }

    public List<RewardTransacao> ObterHistorico(int utilizadorId)
    {
        return _rewardTransacaoRepository.ObterHistorico(utilizadorId);
    }
}
