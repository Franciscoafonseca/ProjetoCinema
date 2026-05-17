using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class RewardTransacaoRepository : IRewardTransacaoRepository
{
    private readonly AppDbContext _context;

    public RewardTransacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RewardTransacao transacao)
    {
        await _context.RewardsTransacoes.AddAsync(transacao);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
