using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IGeneroService
{
    Task<IEnumerable<Genero>> GetAllAsync();

    Task<Genero> CreateAsync(Genero genero);
}
