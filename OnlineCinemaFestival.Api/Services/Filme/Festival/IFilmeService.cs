using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IFilmeService
{
    Task<IEnumerable<FilmeReadDto>> GetAllFilmesAsync();

    Task<IEnumerable<FilmeReadDto>> SearchFilmesTmdbAsync(string query);

    Task<FilmeReadDto> ImportFilmeFromTmdbAsync(int tmdbId);
}
