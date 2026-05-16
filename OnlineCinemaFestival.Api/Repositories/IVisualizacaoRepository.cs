using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IVisualizacaoRepository
{
    Task<Filme?> ObterFilmePorIdAsync(int filmeId);

    Task<Sessao?> ObterSessaoPorIdAsync(int sessaoId);

    Task<bool> FilmePertenceAoFestivalAsync(int filmeId, int festivalId);

    Task<bool> TemAcessoAtivoParaFilmeAsync(int utilizadorId, int filmeId, DateTime agora);

    Task<bool> TemPasseAtivoParaFilmeNoFestivalAsync(
        int utilizadorId,
        int filmeId,
        int festivalId,
        DateTime agora
    );

    Task<bool> TemAcessoAtivoParaSessaoAsync(int utilizadorId, Sessao sessao, DateTime agora);

    Task AddAsync(Visualizacao visualizacao);

    Task AddRangeAsync(IEnumerable<Visualizacao> visualizacoes);

    Task<IEnumerable<Visualizacao>> ObterPorUtilizadorIdAsync(int utilizadorId);

    Task SaveChangesAsync();
}
