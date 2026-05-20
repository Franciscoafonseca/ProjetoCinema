using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IRewardsQueryService
{
    int ObterSaldo(int utilizadorId);
    List<RewardTransacao> ObterHistorico(int utilizadorId);
}
