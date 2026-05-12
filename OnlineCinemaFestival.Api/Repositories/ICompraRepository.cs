using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface ICompraRepository
{
    Task AddAsync(Compra compra);

    Task<Compra?> GetByIdAsync(int id);

    Task<IEnumerable<Compra>> GetByUtilizadorIdAsync(int utilizadorId);

    Task SaveChangesAsync();
}
