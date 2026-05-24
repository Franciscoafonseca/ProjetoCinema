using System.ComponentModel.DataAnnotations;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class CriarReporteUtilizadorDTO
{
    [Required(ErrorMessage = "Indica o motivo do reporte.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "O motivo deve ter entre 10 e 500 caracteres.")]
    public string Motivo { get; set; } = string.Empty;
}

public class AtualizarEstadoReporteUtilizadorDTO
{
    public EstadoReporteUtilizador Estado { get; set; }
}

public class ReporteUtilizadorReadDTO
{
    public int Id { get; set; }

    public EstadoReporteUtilizador Estado { get; set; }

    public string Motivo { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; }

    public DateTime? AtualizadoEm { get; set; }

    public int UtilizadorReportadoId { get; set; }

    public string UtilizadorReportadoNome { get; set; } = string.Empty;

    public int ReportadoPorUtilizadorId { get; set; }

    public string ReportadoPorUtilizadorNome { get; set; } = string.Empty;
}
