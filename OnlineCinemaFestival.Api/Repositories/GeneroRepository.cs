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

    public async Task<List<Genero>> GetAllAsync()
    {
        return await _context.Generos.OrderBy(g => g.Name).ToListAsync();
    }

    public async Task<List<Genero>> GetByIdsAsync(List<int> ids)
    {
        return await _context.Generos.Where(g => ids.Contains(g.Id)).ToListAsync();
    }
}
