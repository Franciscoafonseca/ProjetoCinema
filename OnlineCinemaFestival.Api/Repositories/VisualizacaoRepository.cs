using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class VisualizacaoRepository : IVisualizacaoRepository
{
    private readonly AppDbContext _context;

    public VisualizacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Filme?> ObterFilmePorIdAsync(int filmeId)
    {
        return await _context.Filmes.AsNoTracking().FirstOrDefaultAsync(f => f.Id == filmeId);
    }

    public async Task<Sessao?> ObterSessaoPorIdAsync(int sessaoId)
    {
        return await _context
            .Sessoes.AsSplitQuery()
            .Include(s => s.Festival)
            .Include(s => s.Filme)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sessaoId);
    }

    public async Task<bool> FilmePertenceAoFestivalAsync(int filmeId, int festivalId)
    {
        return await _context.FestivalFilmes.AnyAsync(ff =>
            ff.FilmeId == filmeId && ff.FestivalId == festivalId
        );
    }

    public async Task<IReadOnlySet<int>> ObterFestivalIdsDoFilmeAsync(int filmeId)
    {
        return await _context.FestivalFilmes.AsNoTracking()
            .Where(ff => ff.FilmeId == filmeId)
            .Select(ff => ff.FestivalId)
            .ToHashSetAsync();
    }

    public async Task AddAsync(Visualizacao visualizacao)
    {
        await _context.Visualizacoes.AddAsync(visualizacao);
    }

    public async Task AddRangeAsync(IEnumerable<Visualizacao> visualizacoes)
    {
        await _context.Visualizacoes.AddRangeAsync(visualizacoes);
    }

    public async Task<IEnumerable<Visualizacao>> ObterPorUtilizadorIdAsync(int utilizadorId)
    {
        return await _context
            .Visualizacoes.AsSplitQuery()
            .Where(v => v.UtilizadorId == utilizadorId)
            .Include(v => v.Filme)
            .Include(v => v.Sessao)
            .Include(v => v.Festival)
            .OrderByDescending(v => v.VisualizadoEm)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
