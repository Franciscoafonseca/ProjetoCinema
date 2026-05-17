using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IRewardsQueryService
{
    int GetSaldo(int utilizadorId);
    List<RewardTransacao> GetHistorico(int utilizadorId);
}
