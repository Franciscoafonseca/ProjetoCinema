using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IFestivalFilmeRepository
{
    Task<bool> ExistsAsync(int festivalId, int filmeId);

    Task<FestivalFilme?> GetAsync(int festivalId, int filmeId);

    Task AddAsync(FestivalFilme festivalFilme);

    void Remove(FestivalFilme festivalFilme);

    Task<IEnumerable<Filme>> GetFilmesByFestivalIdAsync(int festivalId);

    Task SaveChangesAsync();
}
