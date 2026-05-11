using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IComunidadeRepository
{
    Task<Comunidade> AddComunidadeAsync(Comunidade comunidade);
    Task<Comunidade?> GetComunidadeByIdAsync(int id);
    Task<IEnumerable<Comunidade>> GetAllComunidadesAsync();
}