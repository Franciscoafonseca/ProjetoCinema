using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IGeneroRepository
{
    Task<List<Genero>> GetAllAsync();

    Task<List<Genero>> GetByIdsAsync(List<int> ids);
}
