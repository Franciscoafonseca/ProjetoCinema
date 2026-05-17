using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Extensions;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api")]
public class PremiosFestivalController : ControllerBase
{
    private readonly IPremioFestivalService _service;

    public PremiosFestivalController(IPremioFestivalService service)
    {
        _service = service;
    }

    [HttpGet("festivals/{festivalId:int}/premios")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PremioFestivalReadDTO>>> ObterPremios(
        int festivalId
    )
    {
        try
        {
            var incluirRascunhos = User.IsInRole(NomesPapeis.Administrador);
            var premios = await _service.ObterPremiosPorFestivalAsync(
                festivalId,
                incluirRascunhos
            );

            return Ok(premios);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("festivals/{festivalId:int}/premios/resultados-publicos")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ResultadoPremioFestivalDTO>>> ObterResultadosFestival(
        int festivalId
    )
    {
        var resultados = await _service.ObterResultadosPublicosAsync(festivalId: festivalId);
        return Ok(resultados);
    }

    [HttpGet("filmes/{filmeId:int}/premios-resultados")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ResultadoPremioFestivalDTO>>> ObterResultadosFilme(
        int filmeId
    )
    {
        var resultados = await _service.ObterResultadosPublicosAsync(filmeId: filmeId);
        return Ok(resultados);
    }

    [HttpPost("festivals/{festivalId:int}/premios")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<PremioFestivalReadDTO>> CriarPremio(
        int festivalId,
        CriarPremioFestivalDTO dto
    )
    {
        try
        {
            var premio = await _service.CriarPremioAsync(festivalId, dto);
            return CreatedAtAction(nameof(ObterPremios), new { festivalId }, premio);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("premios-festival/{premioFestivalId:int}/abrir")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<PremioFestivalReadDTO>> AbrirVotacao(int premioFestivalId)
    {
        try
        {
            return Ok(await _service.AbrirVotacaoAsync(premioFestivalId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("premios-festival/{premioFestivalId:int}/votos")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<IActionResult> Votar(int premioFestivalId, VotarPremioFestivalDTO dto)
    {
        try
        {
            await _service.VotarAsync(premioFestivalId, dto.FilmeId, User.GetUserId());
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("premios-festival/{premioFestivalId:int}/fechar")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<PremioFestivalReadDTO>> FecharVotacao(int premioFestivalId)
    {
        try
        {
            return Ok(await _service.FecharVotacaoAsync(premioFestivalId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("premios-festival/{premioFestivalId:int}/publicar")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<ResultadoPremioFestivalDTO>> PublicarResultados(
        int premioFestivalId
    )
    {
        try
        {
            return Ok(await _service.PublicarResultadosAsync(premioFestivalId, User.GetUserId()));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
