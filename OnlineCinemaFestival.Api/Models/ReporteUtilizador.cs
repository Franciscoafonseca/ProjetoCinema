using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class ReporteUtilizador
{
    public int Id { get; set; }

    public int UtilizadorReportadoId { get; set; }
    public Utilizador UtilizadorReportado { get; set; } = null!;

    public int ReportadoPorUtilizadorId { get; set; }
    public Utilizador ReportadoPorUtilizador { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Motivo { get; set; } = string.Empty;

    public EstadoReporteUtilizador Estado { get; set; } = EstadoReporteUtilizador.Pendente;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public DateTime? AtualizadoEm { get; set; }
}

public enum EstadoReporteUtilizador
{
    Pendente = 0,
    Analisado = 1,
    Rejeitado = 2,
    AcaoAplicada = 3,
}

public enum EstadoModeracaoComentario
{
    Visivel = 0,
    Oculto = 1,
    Removido = 2,
}

public enum AcaoModeracaoComentario
{
    Ocultar = 0,
    Remover = 1,
    Restaurar = 2,
}
