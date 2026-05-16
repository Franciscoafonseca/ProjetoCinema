using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface ICompraRepository
{
    Task AddAsync(Compra compra);
    Task<List<Compra>> GetByUtilizadorAsync(string utilizadorId);
    Task SaveChangesAsync();
}
