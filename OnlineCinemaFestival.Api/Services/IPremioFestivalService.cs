using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IPremioFestivalService
{
    Task<PremioFestivalReadDTO> CriarPremioAsync(int festivalId, CriarPremioFestivalDTO dto);

    Task<PremioFestivalReadDTO> AbrirVotacaoAsync(int premioFestivalId);

    Task VotarAsync(int premioFestivalId, int filmeId, int utilizadorId);

    Task<PremioFestivalReadDTO> FecharVotacaoAsync(int premioFestivalId);

    Task<ResultadoPremioFestivalDTO> PublicarResultadosAsync(
        int premioFestivalId,
        int publicadoPorUtilizadorId
    );

    Task<IEnumerable<ResultadoPremioFestivalDTO>> ObterResultadosPublicosAsync(
        int? festivalId = null,
        int? filmeId = null
    );

    Task<IEnumerable<PremioFestivalReadDTO>> ObterPremiosPorFestivalAsync(
        int festivalId,
        bool incluirRascunhos
    );
}
