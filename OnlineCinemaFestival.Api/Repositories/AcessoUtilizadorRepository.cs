using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class AcessoUtilizadorRepository : IAcessoUtilizadorRepository
{
    private readonly AppDbContext _context;

    public AcessoUtilizadorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<AcessoUtilizador> acessos)
    {
        await _context.AcessosUtilizador.AddRangeAsync(acessos);
    }

    public async Task<IEnumerable<AcessoUtilizador>> ObterPorUtilizadorIdAsync(int utilizadorId)
    {
        return await _context
            .AcessosUtilizador.Where(a => a.UtilizadorId == utilizadorId)
            .Include(a => a.Acesso)
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.Filme)
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.Festival)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<AcessoUtilizador>> ObterAtivosPorUtilizadorIdAsync(
        int utilizadorId,
        DateTime dataAtual
    )
    {
        return await _context
            .AcessosUtilizador.Where(a =>
                a.UtilizadorId == utilizadorId
                && a.Ativo
                && a.InicioValidade <= dataAtual
                && a.FimValidade >= dataAtual
            )
            .Include(a => a.Acesso)
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.Filme)
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.Festival)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .OrderBy(a => a.FimValidade)
            .ToListAsync();
    }

    public async Task<bool> ExisteAcessoAtivoAsync(
        int utilizadorId,
        int acessoId,
        DateTime dataAtual
    )
    {
        return await _context.AcessosUtilizador.AnyAsync(a =>
            a.UtilizadorId == utilizadorId
            && a.AcessoId == acessoId
            && a.Ativo
            && a.FimValidade > dataAtual
        );
    }

    public async Task<AcessoUtilizador?> ObterAcessoValidoAsync(
        int utilizadorId,
        TipoAcesso tipoAcesso,
        DateTime dataAtual,
        int? filmeId = null,
        int? sessaoId = null,
        int? festivalId = null
    )
    {
        var query = _context
            .AcessosUtilizador.AsNoTracking()
            .Where(a =>
                a.UtilizadorId == utilizadorId
                && a.Ativo
                && a.TipoAcesso == tipoAcesso
                && a.InicioValidade <= dataAtual
                && a.FimValidade >= dataAtual
            );

        if (filmeId.HasValue)
            query = query.Where(a => a.FilmeId == filmeId.Value);

        if (sessaoId.HasValue)
            query = query.Where(a => a.SessaoId == sessaoId.Value);

        if (festivalId.HasValue)
            query = query.Where(a => a.FestivalId == festivalId.Value);

        return await query.OrderByDescending(a => a.FimValidade).FirstOrDefaultAsync();
    }

    public async Task<AcessoUtilizador?> ObterPasseCompletoValidoParaFilmeAsync(
        int utilizadorId,
        int filmeId,
        int? festivalId,
        DateTime dataAtual
    )
    {
        var query = _context
            .AcessosUtilizador.AsNoTracking()
            .Where(a =>
                a.UtilizadorId == utilizadorId
                && a.Ativo
                && a.TipoAcesso == TipoAcesso.PasseCompleto
                && a.FestivalId != null
                && a.InicioValidade <= dataAtual
                && a.FimValidade >= dataAtual
            );

        if (festivalId.HasValue)
            query = query.Where(a => a.FestivalId == festivalId.Value);

        return await query
            .Where(a =>
                _context.FestivalFilmes.Any(ff =>
                    ff.FestivalId == a.FestivalId && ff.FilmeId == filmeId
                )
            )
            .OrderByDescending(a => a.FimValidade)
            .FirstOrDefaultAsync();
    }
}
