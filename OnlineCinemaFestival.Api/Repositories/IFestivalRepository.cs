using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IFestivalRepository
{
    Task<IEnumerable<Festival>> GetAllAsync();

    Task<Festival?> GetByIdAsync(int id);

    Task<Festival?> GetDetalheByIdAsync(int id);

    Task AddAsync(Festival festival);

    void Remove(Festival festival);

    Task SaveChangesAsync();
}
