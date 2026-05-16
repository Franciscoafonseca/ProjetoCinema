using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class FestivalFilmeRepository : IFestivalFilmeRepository
{
    private readonly AppDbContext _context;

    public FestivalFilmeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int festivalId, int filmeId)
    {
        return await _context.FestivalFilmes.AnyAsync(ff =>
            ff.FestivalId == festivalId && ff.FilmeId == filmeId
        );
    }

    public async Task<FestivalFilme?> GetAsync(int festivalId, int filmeId)
    {
        return await _context.FestivalFilmes.FirstOrDefaultAsync(ff =>
            ff.FestivalId == festivalId && ff.FilmeId == filmeId
        );
    }

    public async Task AddAsync(FestivalFilme festivalFilme)
    {
        await _context.FestivalFilmes.AddAsync(festivalFilme);
    }

    public void Remove(FestivalFilme festivalFilme)
    {
        _context.FestivalFilmes.Remove(festivalFilme);
    }

    public async Task<IEnumerable<Filme>> ObterFilmesPorFestivalIdAsync(int festivalId)
    {
        return await _context
            .FestivalFilmes.Where(ff => ff.FestivalId == festivalId)
            .Include(ff => ff.Filme)
            .Select(ff => ff.Filme)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
