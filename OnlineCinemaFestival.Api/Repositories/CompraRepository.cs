using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class CompraRepository : ICompraRepository
{
    private readonly AppDbContext _context;

    public CompraRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Compra compra)
    {
        await _context.Compras.AddAsync(compra);
    }

    public async Task<Compra?> ObterPorIdAsync(int id)
    {
        return await _context
            .Compras.Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Sessao)
                        .ThenInclude(s => s!.Filme)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Sessao)
                        .ThenInclude(s => s!.Festival)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Festival)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Filme)
            .Include(c => c.AcessosUtilizador)
            .Include(c => c.Pagamento)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Compra>> ObterPorUtilizadorIdAsync(int utilizadorId)
    {
        return await _context
            .Compras.Where(c => c.UtilizadorId == utilizadorId)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Sessao)
                        .ThenInclude(s => s!.Filme)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Sessao)
                        .ThenInclude(s => s!.Festival)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Festival)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Filme)
            .Include(c => c.AcessosUtilizador)
            .Include(c => c.Pagamento)
            .OrderByDescending(c => c.CriadaEm)
            .ToListAsync();
    }

    public async Task<List<Compra>> ObterHistoricoPorUtilizadorAsync(int utilizadorId)
    {
        return await _context
            .Compras.Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Sessao)
                        .ThenInclude(s => s!.Filme)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Sessao)
                        .ThenInclude(s => s!.Festival)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Festival)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Filme)
            .Include(c => c.AcessosUtilizador)
            .Where(c => c.UtilizadorId == utilizadorId)
            .OrderByDescending(c => c.CriadaEm)
            .ToListAsync();
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var result = await action();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
