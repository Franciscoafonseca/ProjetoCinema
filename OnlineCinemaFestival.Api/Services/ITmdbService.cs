using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ITmdbService
{
    Task<IEnumerable<TmdbFilmeDto>> SearchFilmesTmdbAsync(string query);
    Task<TmdbFilmeDto?> GetFilmeByTmdbIdAsync(int tmdbId);
}