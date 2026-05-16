using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface ICompraRepository
{
    Task AddAsync(Compra compra);

    Task<Compra?> ObterPorIdAsync(int id);

    Task<IEnumerable<Compra>> ObterPorUtilizadorIdAsync(int utilizadorId);

    Task SaveChangesAsync();
}
