using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class AcessoRepository : IAcessoRepository
{
    private readonly AppDbContext _context;

    public AcessoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Acesso>> GetAllAsync()
    {
        return await _context
            .Acessos.Include(a => a.Sessao)
                .ThenInclude(s => s.Festival)
            .Include(a => a.Sessao)
                .ThenInclude(s => s.Filme)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .AsNoTracking()
            .OrderBy(a => a.Tipo)
            .ThenBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Acesso?> GetByIdAsync(int id)
    {
        return await _context
            .Acessos.Include(a => a.Sessao)
                .ThenInclude(s => s.Festival)
            .Include(a => a.Sessao)
                .ThenInclude(s => s.Filme)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(Acesso acesso)
    {
        await _context.Acessos.AddAsync(acesso);
    }

    public void Remove(Acesso acesso)
    {
        _context.Remove(acesso);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
