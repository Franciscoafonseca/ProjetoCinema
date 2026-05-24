using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IRewardTransacaoRepository
{
    Task AddAsync(RewardTransacao transacao);
    Task<bool> ExisteChaveAcaoAsync(int utilizadorId, string chaveAcao);
    List<RewardTransacao> ObterHistorico(int utilizadorId);
    Task SaveChangesAsync();
}
