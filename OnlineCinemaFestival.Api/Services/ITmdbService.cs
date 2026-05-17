using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ITmdbService
{
    Task<IEnumerable<TmdbFilmeDTO>> SearchFilmesTmdbAsync(string query);
    Task<IEnumerable<TmdbFilmeDTO>> ObterFilmesIniciaisAsync();
    Task<TmdbFilmeDTO?> ObterFilmePorTmdbIdAsync(int tmdbId);
    Task<IEnumerable<string>> ObterAtoresAsync(int tmdbId);
    Task<string?> ObterRealizadorAsync(int tmdbId);
    Task<IEnumerable<TmdbReviewDTO>> ObterAvaliacoesExternasAsync(int tmdbId);
    Task<IEnumerable<TmdbGeneroDTO>> ObterGenerosAsync();
    Task<string?> ObterTrailerUrlAsync(int tmdbId);
    int? ConverterDuracaoIso8601ParaSegundos(string? duracaoIso8601);
}
