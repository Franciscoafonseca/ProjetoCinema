using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class GeneroRepository : IGeneroRepository
{
    private readonly AppDbContext _context;

    public GeneroRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Genero>> GetAllAsync()
    {
        return await _context.Generos.AsNoTracking().OrderBy(g => g.Name).ToListAsync();
    }

    public async Task<Genero?> GetByIdAsync(int id)
    {
        return await _context.Generos.FindAsync(id);
    }

    public async Task AddAsync(Genero genero)
    {
        await _context.Generos.AddAsync(genero);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Genero>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var idsDistintos = ids.Distinct().ToList();

        return await _context.Generos.Where(g => idsDistintos.Contains(g.Id)).ToListAsync();
    }
}
