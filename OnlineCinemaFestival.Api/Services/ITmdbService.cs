using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ITmdbService
{
    Task<IEnumerable<TmdbFilmeDto>> SearchFilmesTmdbAsync(string query);
    Task<IEnumerable<TmdbFilmeDto>> GetFilmesIniciaisAsync();
    Task<TmdbFilmeDto?> GetFilmeByTmdbIdAsync(int tmdbId);
    Task<IEnumerable<string>> GetAtoresAsync(int tmdbId);
    Task<string?> GetRealizadorAsync(int tmdbId);
    Task<IEnumerable<TmdbReviewDto>> GetReviewsAsync(int tmdbId);
    Task<IEnumerable<TmdbGeneroDto>> GetGenerosAsync();
    Task<string?> GetTrailerUrlAsync(int tmdbId);
}
