using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public interface IImportacaoFilmeService
{
    Task<List<FilmeDTO>> ObterFilmesIniciaisTmdbAsync();
    Task<List<FilmeDTO>> PesquisarTmdbAsync(string termo);
    Task<FilmeDTO> ImportarTmdbAsync(int tmdbId);
}
