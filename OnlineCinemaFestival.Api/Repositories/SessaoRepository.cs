using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class SessaoRepository : ISessaoRepository
{
    private readonly AppDbContext _context;

    public SessaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sessao>> GetAllAsync()
    {
        return await _context
            .Sessoes.Include(s => s.Festival)
            .Include(s => s.FilmesDaSessao)
                .ThenInclude(sf => sf.Filme)
            .AsNoTracking()
            .OrderBy(s => s.Inicio)
            .ToListAsync();
    }

    public async Task<Sessao?> GetByIdAsync(int id)
    {
        return await _context
            .Sessoes.Include(s => s.Festival)
            .Include(s => s.FilmesDaSessao)
                .ThenInclude(sf => sf.Filme)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sessao>> GetByFestivalIdAsync(int festivalId)
    {
        return await _context
            .Sessoes.Where(s => s.FestivalId == festivalId)
            .Include(s => s.Festival)
            .Include(s => s.FilmesDaSessao)
                .ThenInclude(sf => sf.Filme)
            .AsNoTracking()
            .OrderBy(s => s.Inicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sessao>> GetByFilmeIdAsync(int filmeId)
    {
        return await _context
            .Sessoes.Where(s => s.FilmesDaSessao.Any(sf => sf.FilmeId == filmeId))
            .Include(s => s.Festival)
            .Include(s => s.FilmesDaSessao)
                .ThenInclude(sf => sf.Filme)
            .AsNoTracking()
            .OrderBy(s => s.Inicio)
            .ToListAsync();
    }

    public async Task<bool> HasOverlapAsync(
        int festivalId,
        IEnumerable<int> filmeIds,
        DateTime inicio,
        DateTime fim,
        int? ignoreSessaoId = null
    )
    {
        var idsFilmes = filmeIds.Distinct().ToList();

        var query = _context.Sessoes.Where(s =>
            s.FestivalId == festivalId
            && s.Inicio < fim
            && inicio < s.Fim
            && s.FilmesDaSessao.Any(sf => idsFilmes.Contains(sf.FilmeId))
        );

        if (ignoreSessaoId.HasValue)
            query = query.Where(s => s.Id != ignoreSessaoId.Value);

        return await query.AnyAsync();
    }

    public async Task AddAsync(Sessao sessao)
    {
        await _context.Sessoes.AddAsync(sessao);
    }

    public void Remove(Sessao sessao)
    {
        _context.Sessoes.Remove(sessao);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
