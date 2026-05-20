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

    public async Task<IEnumerable<Sessao>> ObterTodosAsync()
    {
        return await _context
            .Sessoes.Include(s => s.Festival)
            .Include(s => s.Filme)
            .Include(s => s.Acessos)
            .AsNoTracking()
            .OrderBy(s => s.Inicio)
            .ToListAsync();
    }

    public async Task<Sessao?> ObterPorIdAsync(int id)
    {
        return await _context
            .Sessoes.Include(s => s.Festival)
            .Include(s => s.Filme)
            .Include(s => s.Acessos)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sessao>> ObterPorFestivalIdAsync(int festivalId)
    {
        return await _context
            .Sessoes.Where(s => s.FestivalId == festivalId)
            .Include(s => s.Festival)
            .Include(s => s.Filme)
            .Include(s => s.Acessos)
            .AsNoTracking()
            .OrderBy(s => s.Inicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sessao>> ObterPorFilmeIdAsync(int filmeId)
    {
        return await _context
            .Sessoes.Where(s => s.FilmeId == filmeId)
            .Include(s => s.Festival)
            .Include(s => s.Filme)
            .Include(s => s.Acessos)
            .AsNoTracking()
            .OrderBy(s => s.Inicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sessao>> ObterDisponiveisAsync(DateTime dataAtual)
    {
        return await _context
            .Sessoes.Where(s => s.Fim >= dataAtual)
            .Include(s => s.Festival)
            .Include(s => s.Filme)
            .Include(s => s.Acessos)
            .AsNoTracking()
            .OrderBy(s => s.Inicio)
            .ToListAsync();
    }

    public async Task<bool> HasOverlapAsync(
        int festivalId,
        int filmeId,
        DateTime inicio,
        DateTime fim,
        int? ignoreSessaoId = null
    )
    {
        var query = _context.Sessoes.Where(s =>
            s.FestivalId == festivalId
            && s.FilmeId == filmeId
            && s.Inicio < fim
            && inicio < s.Fim
        );

        if (ignoreSessaoId.HasValue)
            query = query.Where(s => s.Id != ignoreSessaoId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> HasAcessosAssociadosAsync(int sessaoId)
    {
        return await _context.Acessos.AnyAsync(a => a.SessaoId == sessaoId)
            || await _context.AcessosUtilizador.AnyAsync(a => a.SessaoId == sessaoId);
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
