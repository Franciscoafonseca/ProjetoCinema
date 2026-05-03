using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class AcessoRepository
{
    private readonly AppDbContext _context;

    public AcessoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Acesso acesso)
    {
        _context.Acessos.Add(acesso);
        await _context.SaveChangesAsync();
    }

    public async Task AddManyAsync(List<Acesso> acessos)
    {
        _context.Acessos.AddRange(acessos);
        await _context.SaveChangesAsync();
    }

    public async Task<Acesso?> GetByIdAsync(int id)
    {
        return await _context.Acessos.FindAsync(id);
    }

    public async Task<List<Acesso>> GetByUtilizadorIdAsync(string utilizadorId)
    {
        return await _context.Acessos
            .Where(a => a.UtilizadorId == utilizadorId)
            .ToListAsync();
    }
}
