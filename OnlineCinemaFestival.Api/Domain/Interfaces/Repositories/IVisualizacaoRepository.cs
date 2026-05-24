using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IVisualizacaoRepository
{
    Task<Filme?> ObterFilmePorIdAsync(int filmeId);

    Task<Sessao?> ObterSessaoPorIdAsync(int sessaoId);

    Task<bool> FilmePertenceAoFestivalAsync(int filmeId, int festivalId);

    Task<IReadOnlySet<int>> ObterFestivalIdsDoFilmeAsync(int filmeId);

    Task AddAsync(Visualizacao visualizacao);

    Task AddRangeAsync(IEnumerable<Visualizacao> visualizacoes);

    Task<IEnumerable<Visualizacao>> ObterPorUtilizadorIdAsync(int utilizadorId);

    Task SaveChangesAsync();
}
