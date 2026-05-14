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
    public async Task<ActionResult<IEnumerable<FilmeReadDto>>> GetFilmesByFestival(int festivalId)
    {
        try
        {
            var filmes = await _service.GetFilmesByFestivalAsync(festivalId);

            return Ok(filmes);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> AssociarFilme(int festivalId, AssociarFilmeFestivalDto dto)
    {
        try
        {
            await _service.AssociarFilmeAsync(festivalId, dto.FilmeId);

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
