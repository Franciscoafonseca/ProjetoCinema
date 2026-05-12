using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IVisualizacaoRepository
{
    Task<Filme?> GetFilmeByIdAsync(int filmeId);

    Task<Sessao?> GetSessaoByIdAsync(int sessaoId);

    Task<bool> FilmePertenceAoFestivalAsync(int filmeId, int festivalId);

    Task<bool> TemAcessoAtivoParaFilmeAsync(int utilizadorId, int filmeId, DateTime agora);

    Task<bool> TemPasseAtivoParaFilmeNoFestivalAsync(
        int utilizadorId,
        int filmeId,
        int festivalId,
        DateTime agora
    );

    Task<bool> TemAcessoAtivoParaSessaoAsync(int utilizadorId, Sessao sessao, DateTime agora);
}
