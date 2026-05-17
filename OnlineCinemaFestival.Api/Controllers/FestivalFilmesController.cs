using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/festivals/{festivalId:int}/filmes")]
public class FestivalFilmesController : ControllerBase
{
    private readonly IFestivalFilmeService _service;

    public FestivalFilmesController(IFestivalFilmeService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FilmeResumoDTO>>> GetFilmesByFestival(int festivalId)
    {
        try
        {
            var filmes = await _service.ObterFilmesPorFestivalAsync(festivalId);

            return Ok(filmes);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("associacoes")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FestivalFilmeReadDTO>>> GetAssociacoesByFestival(
        int festivalId
    )
    {
        try
        {
            var associacoes = await _service.ObterAssociacoesPorFestivalAsync(festivalId);

            return Ok(associacoes);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<FestivalFilmeReadDTO>> AssociarFilme(
        int festivalId,
        AssociarFilmeFestivalDTO dto
    )
    {
        try
        {
            var associacao = await _service.AssociarFilmeAsync(festivalId, dto);

            return CreatedAtAction(nameof(GetAssociacoesByFestival), new { festivalId }, associacao);
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

    [HttpDelete("{filmeId:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> RemoverFilme(int festivalId, int filmeId)
    {
        try
        {
            await _service.RemoverFilmeAsync(festivalId, filmeId);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
