using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class RewardsRepository
{
    private readonly AppDbContext _context;

    public RewardsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddOrUpdatePointsAsync(string utilizadorId, int pontosGanhos)
    {
        var reward = await _context.Rewards.FirstOrDefaultAsync(r => r.UtilizadorId == utilizadorId);

        if (reward == null)
        {
            reward = new Reward
            {
                UtilizadorId = utilizadorId,
                Pontos = pontosGanhos
            };
            _context.Rewards.Add(reward);
        }
        else
        {
            reward.Pontos += pontosGanhos;
            _context.Rewards.Update(reward);
        }

        await _context.SaveChangesAsync();
    }
}
