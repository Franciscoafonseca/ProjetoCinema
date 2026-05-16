using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IFilmeService
{
    Task<IEnumerable<FilmeReadDto>> GetAllFilmesAsync();

    Task<IEnumerable<FilmeReadDto>> SearchFilmesTmdbAsync(string query);

    Task<IEnumerable<FilmeReadDto>> GetFilmesIniciaisTmdbAsync();

    Task<FilmeReadDto> ImportFilmeFromTmdbAsync(int tmdbId);

    Task<FilmeDetalheDto?> GetDetalheAsync(int filmeId, int? utilizadorId);

    Task<AvaliacaoDto> CriarReviewAsync(int utilizadorId, int filmeId, CriarAvaliacaoDto dto);
}
