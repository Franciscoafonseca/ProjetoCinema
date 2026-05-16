using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IRewardsQueryService
{
    int GetSaldo(string utilizadorId);
    List<RewardTransacao> GetHistorico(string utilizadorId);
}
