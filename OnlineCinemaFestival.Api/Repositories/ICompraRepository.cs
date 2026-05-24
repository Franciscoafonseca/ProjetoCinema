using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface ICompraRepository
{
    Task AddAsync(Compra compra);

    Task<Compra?> ObterPorIdAsync(int id);

    Task<IEnumerable<Compra>> ObterPorUtilizadorIdAsync(int utilizadorId);

    Task<List<Compra>> ObterHistoricoPorUtilizadorAsync(int utilizadorId);

    Task<List<Compra>> ObterPagamentosMultibancoPorUtilizadorAsync(int utilizadorId);

    Task<List<Compra>> ObterPagamentosMultibancoPendentesAsync();

    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);

    Task SaveChangesAsync();
}
