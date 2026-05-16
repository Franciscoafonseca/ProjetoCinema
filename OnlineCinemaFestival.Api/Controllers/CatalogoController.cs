using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/catalogo")]
[AllowAnonymous]
public class CatalogoController : ControllerBase
{
    private readonly ICatalogoService _service;

    public CatalogoController(ICatalogoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FilmeResumoDTO>>> GetCatalogo(
        [FromQuery] CatalogoQueryDTO query
    )
    {
        try
        {
            var filmes = await _service.ObterCatalogoAsync(query);

            return Ok(filmes);
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

    [HttpGet("festival/{festivalId:int}")]
    public async Task<ActionResult<IEnumerable<FilmeResumoDTO>>> GetCatalogoByFestival(
        int festivalId,
        [FromQuery] CatalogoQueryDTO query
    )
    {
        try
        {
            var filmes = await _service.ObterFilmesPorFestivalAsync(festivalId, query);

            return Ok(filmes);
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

    [HttpGet("filmes/{filmeId:int}")]
    public async Task<ActionResult<FilmeDetalheDTO>> GetFilmeDetalhes(int filmeId)
    {
        var filme = await _service.ObterDetalhesFilmeAsync(filmeId);

        if (filme == null)
            return NotFound("Filme não encontrado.");

        return Ok(filme);
    }
}
