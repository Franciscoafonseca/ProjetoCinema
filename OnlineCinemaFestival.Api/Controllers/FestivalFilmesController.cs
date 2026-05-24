using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/festivals/{festivalId:int}/filmes")]
[Route("api/festivais/{festivalId:int}/filmes")]
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
        var filmes = await _service.ObterFilmesPorFestivalAsync(festivalId);

        return Ok(filmes);
    }

    [HttpGet("associacoes")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FestivalFilmeReadDTO>>> GetAssociacoesByFestival(
        int festivalId
    )
    {
        var associacoes = await _service.ObterAssociacoesPorFestivalAsync(festivalId);

        return Ok(associacoes);
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<FestivalFilmeReadDTO>> AssociarFilme(
        int festivalId,
        AssociarFilmeFestivalDTO dto
    )
    {
        var associacao = await _service.AssociarFilmeAsync(festivalId, dto);

        return CreatedAtAction(nameof(GetAssociacoesByFestival), new { festivalId }, associacao);
    }

    [HttpDelete("{filmeId:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> RemoverFilme(int festivalId, int filmeId)
    {
        await _service.RemoverFilmeAsync(festivalId, filmeId);

        return NoContent();
    }
}
