using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IRewardsQueryService
{
    RewardsSaldoReadDto ObterSaldo(int utilizadorId);

    IEnumerable<RewardTransacaoReadDto> ObterHistorico(int utilizadorId);
}
