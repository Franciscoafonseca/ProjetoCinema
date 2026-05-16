using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IFestivalRepository
{
    Task<IEnumerable<Festival>> ObterTodosAsync();

    Task<Festival?> ObterPorIdAsync(int id);

    Task<Festival?> ObterDetalhePorIdAsync(int id);

    Task AddAsync(Festival festival);

    void Remove(Festival festival);

    Task SaveChangesAsync();
}
