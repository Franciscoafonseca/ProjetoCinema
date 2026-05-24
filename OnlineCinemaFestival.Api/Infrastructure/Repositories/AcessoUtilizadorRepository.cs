using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Infrastructure.Data;
using OnlineCinemaFestival.Api.Domain;

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
        return await AcessosUtilizadorComDetalhes()
            .Where(a => a.UtilizadorId == utilizadorId)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<AcessoUtilizador>> ObterAtivosPorUtilizadorIdAsync(
        int utilizadorId,
        DateTime dataAtual
    )
    {
        return await AcessosUtilizadorComDetalhes()
            .Where(a =>
                a.UtilizadorId == utilizadorId
                && a.Ativo
                && a.InicioValidade <= dataAtual
                && a.FimValidade >= dataAtual
            )
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

    public async Task<bool> ExisteParaCompraAsync(int compraId)
    {
        return await _context.AcessosUtilizador.AnyAsync(a => a.CompraId == compraId);
    }

    private IQueryable<AcessoUtilizador> AcessosUtilizadorComDetalhes()
    {
        return _context
            .AcessosUtilizador.AsSplitQuery()
            .Include(a => a.Acesso)
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.Filme)
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.Festival)
            .Include(a => a.Festival)
            .Include(a => a.Filme);
    }
}
