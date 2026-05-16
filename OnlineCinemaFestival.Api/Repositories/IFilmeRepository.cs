using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IFilmeRepository
{
    Task<IEnumerable<Filme>> GetAllAsync();

    Task<Filme?> GetByIdAsync(int id);

    Task<Filme?> GetDetalheByIdAsync(int id);

    Task<Filme?> GetByTmdbIdAsync(int tmdbId);

    Task<List<Filme>> GetTopAsync(int quantidade);

    Task<Genero> ObterOuCriarGeneroAsync(string nome);

    Task<bool> UtilizadorViuFilmeAsync(int utilizadorId, int filmeId);

    Task<Avaliacao?> GetAvaliacaoAsync(int utilizadorId, int filmeId);

    Task AddAvaliacaoAsync(Avaliacao avaliacao);

    Task AddAsync(Filme filme);

    Task SaveChangesAsync();
}
