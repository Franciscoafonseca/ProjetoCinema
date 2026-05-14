using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class FilmeRepository : IFilmeRepository
{
    private readonly AppDbContext _context;

    public FilmeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Filme>> GetAllAsync()
    {
        return await _context.Filmes.AsNoTracking().ToListAsync();
    }

    public async Task<Filme?> GetByIdAsync(int id)
    {
        return await _context.Filmes.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Filme?> GetByTmdbIdAsync(int tmdbId)
    {
        return await _context.Filmes.FirstOrDefaultAsync(f => f.TmdbId == tmdbId);
    }

    public async Task AddAsync(Filme filme)
    {
        await _context.Filmes.AddAsync(filme);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
