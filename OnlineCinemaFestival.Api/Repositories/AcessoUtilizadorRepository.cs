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

    public async Task<IEnumerable<AcessoUtilizador>> GetByUtilizadorIdAsync(int utilizadorId)
    {
        return await _context
            .AcessosUtilizador.Where(a => a.UtilizadorId == utilizadorId)
            .Include(a => a.Acesso)
            .OrderByDescending(a => a.CriadoEm)
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
}
