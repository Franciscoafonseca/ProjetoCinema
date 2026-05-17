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

    public async Task<bool> ExisteAsync(int festivalId, int filmeId)
    {
        return await _context.FestivalFilmes.AnyAsync(ff =>
            ff.FestivalId == festivalId && ff.FilmeId == filmeId
        );
    }

    public async Task<FestivalFilme?> ObterAsync(int festivalId, int filmeId)
    {
        return await _context
            .FestivalFilmes.Include(ff => ff.Filme)
            .FirstOrDefaultAsync(ff => ff.FestivalId == festivalId && ff.FilmeId == filmeId);
    }

    public async Task AdicionarAsync(FestivalFilme festivalFilme)
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

    public async Task<IEnumerable<FestivalFilme>> ObterAssociacoesPorFestivalIdAsync(int festivalId)
    {
        return await _context
            .FestivalFilmes.Where(ff => ff.FestivalId == festivalId)
            .Include(ff => ff.Filme)
            .AsNoTracking()
            .OrderBy(ff => ff.Secao)
            .ThenBy(ff => ff.Categoria)
            .ThenBy(ff => ff.Filme.Titulo)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
