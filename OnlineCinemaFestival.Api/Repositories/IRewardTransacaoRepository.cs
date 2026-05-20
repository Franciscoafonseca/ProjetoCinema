using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IRewardTransacaoRepository
{
    Task AddAsync(RewardTransacao transacao);
    List<RewardTransacao> ObterHistorico(int utilizadorId);
    Task SaveChangesAsync();
}
