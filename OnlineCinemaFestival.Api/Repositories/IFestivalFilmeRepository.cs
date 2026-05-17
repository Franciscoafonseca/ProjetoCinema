using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IFestivalFilmeRepository
{
    Task<bool> ExisteAsync(int festivalId, int filmeId);

    Task<FestivalFilme?> ObterAsync(int festivalId, int filmeId);

    Task AdicionarAsync(FestivalFilme festivalFilme);

    void Remove(FestivalFilme festivalFilme);

    Task<IEnumerable<Filme>> ObterFilmesPorFestivalIdAsync(int festivalId);

    Task<IEnumerable<FestivalFilme>> ObterAssociacoesPorFestivalIdAsync(int festivalId);

    Task SaveChangesAsync();
}
