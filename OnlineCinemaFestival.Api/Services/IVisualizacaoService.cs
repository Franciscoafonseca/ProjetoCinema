using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IVisualizacaoService
{
    Task<VisualizacaoReadDTO> ObterVisualizacaoFilmeAsync(
        int utilizadorId,
        int filmeId,
        int? festivalId
    );

    Task<VisualizacaoReadDTO> ObterVisualizacaoSessaoAsync(int utilizadorId, int sessaoId);

    Task<IEnumerable<VisualizacaoHistoricoReadDTO>> ObterHistoricoDoUtilizadorAsync(
        int utilizadorId
    );

    Task<VisualizacaoHistoricoReadDTO> RegistarVisualizacaoAsync(
        int utilizadorId,
        RegistarVisualizacaoDTO dto
    );
}
