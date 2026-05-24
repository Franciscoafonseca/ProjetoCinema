using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IReporteUtilizadorService
{
    Task<ReporteUtilizadorReadDTO> ReportarAsync(
        int utilizadorReportadoId,
        int reportadoPorUtilizadorId,
        CriarReporteUtilizadorDTO dto
    );

    Task<List<ReporteUtilizadorReadDTO>> ObterTodosAsync();

    Task<ReporteUtilizadorReadDTO> AtualizarEstadoAsync(
        int reporteId,
        EstadoReporteUtilizador estado
    );
}
