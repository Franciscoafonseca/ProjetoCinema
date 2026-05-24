using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ReporteUtilizadorService : IReporteUtilizadorService
{
    private readonly IReporteUtilizadorRepository _reporteRepository;
    private readonly IUtilizadorRepository _utilizadorRepository;

    public ReporteUtilizadorService(
        IReporteUtilizadorRepository reporteRepository,
        IUtilizadorRepository utilizadorRepository
    )
    {
        _reporteRepository = reporteRepository;
        _utilizadorRepository = utilizadorRepository;
    }

    public async Task<ReporteUtilizadorReadDTO> ReportarAsync(
        int utilizadorReportadoId,
        int reportadoPorUtilizadorId,
        CriarReporteUtilizadorDTO dto
    )
    {
        if (utilizadorReportadoId == reportadoPorUtilizadorId)
            throw new InvalidOperationException("Nao podes reportar o teu proprio perfil.");

        var motivo = dto.Motivo.Trim();
        if (motivo.Length < 10)
            throw new ArgumentException("Descreve o motivo do reporte com pelo menos 10 caracteres.");

        var utilizadorReportado = await _utilizadorRepository.ObterPorIdAsync(utilizadorReportadoId);
        if (utilizadorReportado == null)
            throw new KeyNotFoundException("Utilizador reportado nao encontrado.");

        var reportadoPor = await _utilizadorRepository.ObterPorIdAsync(reportadoPorUtilizadorId);
        if (reportadoPor == null)
            throw new KeyNotFoundException("Utilizador autenticado nao encontrado.");

        var jaReportado = await _reporteRepository.ExisteAsync(
            utilizadorReportadoId,
            reportadoPorUtilizadorId
        );
        if (jaReportado)
            throw new InvalidOperationException("Ja reportaste este perfil. A equipa vai analisar o caso.");

        var reporte = new ReporteUtilizador
        {
            UtilizadorReportadoId = utilizadorReportadoId,
            UtilizadorReportado = utilizadorReportado,
            ReportadoPorUtilizadorId = reportadoPorUtilizadorId,
            ReportadoPorUtilizador = reportadoPor,
            Motivo = motivo,
            Estado = EstadoReporteUtilizador.Pendente,
            CriadoEm = DateTime.UtcNow,
        };

        await _reporteRepository.AddAsync(reporte);

        return ToReadDTO(reporte);
    }

    public async Task<List<ReporteUtilizadorReadDTO>> ObterTodosAsync()
    {
        var reportes = await _reporteRepository.ObterTodosAsync();
        return reportes.Select(ToReadDTO).ToList();
    }

    public async Task<ReporteUtilizadorReadDTO> AtualizarEstadoAsync(
        int reporteId,
        EstadoReporteUtilizador estado
    )
    {
        var reporte = await _reporteRepository.ObterPorIdAsync(reporteId);
        if (reporte == null)
            throw new KeyNotFoundException("Reporte nao encontrado.");

        if (!Enum.IsDefined(estado))
            throw new ArgumentException("Estado de reporte invalido.");

        reporte.Estado = estado;
        reporte.AtualizadoEm = DateTime.UtcNow;

        await _reporteRepository.SaveChangesAsync();

        return ToReadDTO(reporte);
    }

    private static ReporteUtilizadorReadDTO ToReadDTO(ReporteUtilizador reporte)
    {
        return new ReporteUtilizadorReadDTO
        {
            Id = reporte.Id,
            Estado = reporte.Estado,
            Motivo = reporte.Motivo,
            CriadoEm = reporte.CriadoEm,
            AtualizadoEm = reporte.AtualizadoEm,
            UtilizadorReportadoId = reporte.UtilizadorReportadoId,
            UtilizadorReportadoNome = reporte.UtilizadorReportado?.Name ?? "Utilizador",
            ReportadoPorUtilizadorId = reporte.ReportadoPorUtilizadorId,
            ReportadoPorUtilizadorNome = reporte.ReportadoPorUtilizador?.Name ?? "Utilizador",
        };
    }
}
