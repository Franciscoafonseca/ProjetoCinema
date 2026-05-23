using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public interface IConsultaFilmeService
{
    Task<List<FilmeDTO>> ObterFilmesAsync(
        string? genero = null,
        string? pesquisa = null,
        int? ordenarPor = null,
        bool descendente = false,
        int? festivalId = null
    );

    Task<FilmeDTO?> ObterFilmePorIdAsync(int id);
    Task<List<string>> ObterGenerosAsync();
}
