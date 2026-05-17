using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IRewardTransacaoRepository
{
    Task AddAsync(RewardTransacao transacao);
    Task SaveChangesAsync();
}
