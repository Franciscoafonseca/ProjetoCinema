using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

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
