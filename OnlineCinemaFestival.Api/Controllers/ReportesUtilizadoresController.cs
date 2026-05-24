using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Common.Auth;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Extensions;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api")]
public class ReportesUtilizadoresController : ControllerBase
{
    private readonly IReporteUtilizadorService _reporteService;

    public ReportesUtilizadoresController(IReporteUtilizadorService reporteService)
    {
        _reporteService = reporteService;
    }

    [Authorize]
    [HttpPost("profiles/{utilizadorId:int}/reportes")]
    public async Task<ActionResult<ReporteUtilizadorReadDTO>> ReportarUtilizador(
        int utilizadorId,
        CriarReporteUtilizadorDTO dto
    )
    {
        var reporte = await _reporteService.ReportarAsync(
            utilizadorId,
            User.GetUserId(),
            dto
        );

        return CreatedAtAction(nameof(ObterReportesAdmin), new { id = reporte.Id }, reporte);
    }

    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    [HttpGet("admin/reportes-utilizadores")]
    public async Task<ActionResult<List<ReporteUtilizadorReadDTO>>> ObterReportesAdmin()
    {
        return Ok(await _reporteService.ObterTodosAsync());
    }

    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    [HttpPatch("admin/reportes-utilizadores/{id:int}/estado")]
    public async Task<ActionResult<ReporteUtilizadorReadDTO>> AtualizarEstado(
        int id,
        AtualizarEstadoReporteUtilizadorDTO dto
    )
    {
        return Ok(await _reporteService.AtualizarEstadoAsync(id, dto.Estado));
    }
}
