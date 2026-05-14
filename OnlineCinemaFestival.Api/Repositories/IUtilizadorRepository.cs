using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IUtilizadorRepository
{
    Task<Utilizador?> GetByIdAsync(int id);

    Task<Utilizador?> GetByEmailAsync(string email);

    Task<Utilizador?> GetWithProfileAsync(int id);

    Task<List<Utilizador>> GetPublicProfilesAsync();

    Task AddAsync(Utilizador utilizador);

    Task SaveChangesAsync();
}
