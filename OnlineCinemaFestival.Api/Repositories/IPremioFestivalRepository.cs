using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IPremioFestivalRepository
{
    Task<bool> FestivalExisteAsync(int festivalId);

    Task<PremioFestival?> ObterPremioAsync(int premioFestivalId);

    Task<PremioFestival?> ObterPremioComResultadoAsync(int premioFestivalId);

    Task AddPremioAsync(PremioFestival premio);

    Task<bool> FilmeElegivelAsync(int festivalId, int filmeId);

    Task<bool> UtilizadorJaVotouAsync(int premioFestivalId, int utilizadorId);

    Task<bool> UtilizadorViuTodosFilmesElegiveisAsync(int festivalId, int utilizadorId);

    Task AddVotoAsync(VotoPremioFestival voto);

    Task<(int FilmeId, int TotalVotos)?> ObterVencedorPorVotosAsync(int premioFestivalId);

    Task AddResultadoAsync(ResultadoPremioFestival resultado);

    Task<ResultadoPremioFestival?> ObterResultadoCompletoAsync(int premioFestivalId);

    Task<List<ResultadoPremioFestival>> ObterResultadosPublicosAsync(int? festivalId, int? filmeId);

    Task<List<PremioFestival>> ObterPremiosPorFestivalAsync(int festivalId, bool incluirRascunhos);

    Task<List<PremioFestival>> ObterPremiosPendentesPublicacaoAsync(DateTime dataAtual);

    Task SaveChangesAsync();

    Task<bool> TrySaveChangesAsync();
}
