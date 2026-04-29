using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IFilmeService
{   
    // Listar todos os filmes
    Task<IEnumerable<FilmeReadDto>> GetAllFilmesAsync();
    // Pesquisar filmes por título ou gênero por exemplo
    Task<IEnumerable<FilmeReadDto>> SearchFilmesTmdbAsync(string query);
    // Buscar um filme no tmdb e importar para a base de dados
    Task<FilmeReadDto> ImportFilmeFromTmdbAsync(int tmdbId);
}