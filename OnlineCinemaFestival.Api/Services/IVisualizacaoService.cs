using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IVisualizacaoService
{
    Task<VisualizacaoReadDto> ObterVisualizacaoFilmeAsync(
        int utilizadorId,
        int filmeId,
        int? festivalId
    );

    Task<VisualizacaoReadDto> ObterVisualizacaoSessaoAsync(int utilizadorId, int sessaoId);

    Task<IEnumerable<VisualizacaoHistoricoReadDto>> ObterHistoricoDoUtilizadorAsync(
        int utilizadorId
    );

    Task<VisualizacaoHistoricoReadDto> RegistarVisualizacaoAsync(
        int utilizadorId,
        RegistarVisualizacaoDto dto
    );
}
