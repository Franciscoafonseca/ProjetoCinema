using Microsoft.EntityFrameworkCore;
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

    public async Task<bool> ExisteChaveAcaoAsync(int utilizadorId, string chaveAcao)
    {
        return await _context.RewardsTransacoes.AnyAsync(t =>
            t.UtilizadorId == utilizadorId && t.ChaveAcao == chaveAcao
        );
    }

    public List<RewardTransacao> ObterHistorico(int utilizadorId)
    {
        return _context
            .RewardsTransacoes.AsNoTracking()
            .Where(t => t.UtilizadorId == utilizadorId)
            .OrderByDescending(t => t.Data)
            .ToList();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
