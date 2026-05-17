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
            .Include(s => s.FilmesDaSessao)
                .ThenInclude(sf => sf.Filme)
            .AsNoTracking()
            .OrderBy(s => s.Inicio)
            .ToListAsync();
    }

    public async Task<Sessao?> ObterPorIdAsync(int id)
    {
        return await _context
            .Sessoes.Include(s => s.Festival)
            .Include(s => s.FilmesDaSessao)
                .ThenInclude(sf => sf.Filme)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sessao>> ObterPorFestivalIdAsync(int festivalId)
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

    public async Task<IEnumerable<Sessao>> ObterPorFilmeIdAsync(int filmeId)
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

    public async Task<IEnumerable<Sessao>> ObterDisponiveisAsync(DateTime dataAtual)
    {
        return await _context
            .Sessoes.Where(s => s.Fim >= dataAtual)
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

    public async Task<bool> HasAcessosAssociadosAsync(int sessaoId)
    {
        return await _context.Acessos.AnyAsync(a => a.SessaoId == sessaoId)
            || await _context.AcessosUtilizador.AnyAsync(a => a.SessaoId == sessaoId);
    }

    public async Task AddAsync(Sessao sessao)
    {
        await _context.Sessoes.AddAsync(sessao);
    }

    public async Task AdicionarFilmeAsync(SessaoFilme sessaoFilme)
    {
        await _context.SessaoFilmes.AddAsync(sessaoFilme);
    }

    public async Task<bool> ExisteFilmeNaSessaoAsync(int sessaoId, int filmeId)
    {
        return await _context.SessaoFilmes.AnyAsync(sf =>
            sf.SessaoId == sessaoId && sf.FilmeId == filmeId
        );
    }

    public async Task<bool> ExisteOrdemNaSessaoAsync(int sessaoId, int ordem)
    {
        return await _context.SessaoFilmes.AnyAsync(sf =>
            sf.SessaoId == sessaoId && sf.Ordem == ordem
        );
    }

    public async Task<int> ObterProximaOrdemAsync(int sessaoId)
    {
        var maiorOrdem = await _context
            .SessaoFilmes.Where(sf => sf.SessaoId == sessaoId)
            .Select(sf => (int?)sf.Ordem)
            .MaxAsync();

        return (maiorOrdem ?? 0) + 1;
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
