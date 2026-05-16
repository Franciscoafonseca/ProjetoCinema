using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IGeneroRepository
{
    Task<IEnumerable<Genero>> ObterTodosAsync();

    Task<Genero?> ObterPorIdAsync(int id);

    Task AddAsync(Genero genero);

    Task SaveChangesAsync();
    Task<List<Genero>> ObterPorIdsAsync(IEnumerable<int> ids);
}
