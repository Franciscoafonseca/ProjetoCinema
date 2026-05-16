using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IFilmeRepository
{
    Task<IEnumerable<Filme>> ObterTodosAsync();

    Task<Filme?> ObterPorIdAsync(int id);

    Task<Filme?> ObterDetalhePorIdAsync(int id);

    Task<Filme?> ObterPorTmdbIdAsync(int tmdbId);

    Task<List<Filme>> ObterPrincipaisAsync(int quantidade);

    Task<Genero> ObterOuCriarGeneroAsync(string nome);

    Task<bool> UtilizadorViuFilmeAsync(int utilizadorId, int filmeId);

    Task<Avaliacao?> ObterAvaliacaoAsync(int utilizadorId, int filmeId);

    Task AddAvaliacaoAsync(Avaliacao avaliacao);

    Task AddAsync(Filme filme);

    Task SaveChangesAsync();
}
