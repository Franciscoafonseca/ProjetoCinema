using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IGeneroRepository
{
    Task<IEnumerable<Genero>> GetAllAsync();

    Task<Genero?> GetByIdAsync(int id);

    Task AddAsync(Genero genero);

    Task SaveChangesAsync();
    Task<List<Genero>> GetByIdsAsync(IEnumerable<int> ids);
}
