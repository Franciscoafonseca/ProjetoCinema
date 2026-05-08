using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IAcessoRepository
{
    Task<IEnumerable<Acesso>> GetAllAsync();

    Task<Acesso?> GetByIdAsync(int id);

    Task AddAsync(Acesso acesso);

    void Remove(Acesso acesso);

    Task SaveChangesAsync();
}
