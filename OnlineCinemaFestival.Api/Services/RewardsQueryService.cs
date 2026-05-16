using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsQueryService : IRewardsQueryService
{
    private readonly AppDbContext _context;

    public RewardsQueryService(AppDbContext context)
    {
        _context = context;
    }

    public int GetSaldo(string utilizadorId)
    {
        var saldo = _context.Rewards.FirstOrDefault(r => r.UtilizadorId == utilizadorId);
        return saldo?.Pontos ?? 0;
    }

    public List<RewardTransacao> GetHistorico(string utilizadorId)
    {
        return _context.RewardsTransacoes
            .Where(t => t.UtilizadorId == utilizadorId)
            .OrderByDescending(t => t.Data)
            .ToList();
    }
}
