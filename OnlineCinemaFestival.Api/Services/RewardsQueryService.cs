using OnlineCinemaFestival.Api.DTOs;
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

    public RewardsSaldoReadDto ObterSaldo(int utilizadorId)
    {
        return new RewardsSaldoReadDto
        {
            UtilizadorId = utilizadorId,
            Pontos = _rewardsRepository.ObterSaldo(utilizadorId),
        };
    }

    public IEnumerable<RewardTransacaoReadDto> ObterHistorico(int utilizadorId)
    {
        return _rewardTransacaoRepository
            .ObterHistorico(utilizadorId)
            .Select(t => new RewardTransacaoReadDto
            {
                Id = t.Id,
                UtilizadorId = t.UtilizadorId,
                Pontos = t.Pontos,
                Data = t.Data,
                Motivo = t.Motivo,
            });
    }
}
