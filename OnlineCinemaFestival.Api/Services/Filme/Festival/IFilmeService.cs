using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IFilmeService
{
    Task<IEnumerable<FilmeReadDTO>> ObterTodosFilmesAsync();

    Task<IEnumerable<FilmeReadDTO>> SearchFilmesTmdbAsync(string query);

    Task<IEnumerable<FilmeReadDTO>> ObterFilmesIniciaisTmdbAsync();

    Task<FilmeReadDTO> ImportFilmeFromTmdbAsync(int tmdbId);

    Task<FilmeReadDTO> AtualizarVideoAsync(int filmeId, AtualizarVideoFilmeDTO dto);

    Task<FilmeDetalheDTO?> ObterDetalheAsync(int filmeId, int? utilizadorId);

    Task<AvaliacaoDTO> CriarReviewAsync(int utilizadorId, int filmeId, CriarAvaliacaoDTO dto);
}
