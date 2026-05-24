using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Infrastructure.Data;
using OnlineCinemaFestival.Api.Domain;

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
        return await ComprasComDetalhes(incluirPagamento: true)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Compra>> ObterPorUtilizadorIdAsync(int utilizadorId)
    {
        return await ComprasComDetalhes(incluirPagamento: true)
            .Where(c => c.UtilizadorId == utilizadorId)
            .OrderByDescending(c => c.CriadaEm)
            .ToListAsync();
    }

    public async Task<List<Compra>> ObterHistoricoPorUtilizadorAsync(int utilizadorId)
    {
        return await ComprasComDetalhes(incluirPagamento: false)
            .Where(c => c.UtilizadorId == utilizadorId)
            .OrderByDescending(c => c.CriadaEm)
            .ToListAsync();
    }

    public async Task<List<Compra>> ObterPagamentosMultibancoPorUtilizadorAsync(int utilizadorId)
    {
        return await ComprasComDetalhes(incluirPagamento: true)
            .Where(c =>
                c.UtilizadorId == utilizadorId
                && c.Pagamento != null
                && c.Pagamento.Metodo == MetodosPagamento.ReferenciaMultibanco
                && (c.Pagamento.Estado == EstadoPagamento.Pendente
                    || c.Pagamento.Estado == EstadoPagamento.Expirado)
            )
            .OrderByDescending(c => c.CriadaEm)
            .ToListAsync();
    }

    public async Task<List<Compra>> ObterPagamentosMultibancoPendentesAsync()
    {
        return await ComprasComDetalhes(incluirPagamento: true)
            .Where(c =>
                c.Pagamento != null
                && c.Pagamento.Metodo == MetodosPagamento.ReferenciaMultibanco
                && c.Pagamento.Estado == EstadoPagamento.Pendente
            )
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

    private IQueryable<Compra> ComprasComDetalhes(bool incluirPagamento)
    {
        var query = _context
            .Compras.AsSplitQuery()
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
            .Include(c => c.AcessosUtilizador);

        return incluirPagamento ? query.Include(c => c.Pagamento) : query;
    }
}
