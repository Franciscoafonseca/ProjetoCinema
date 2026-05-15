using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilmesController : ControllerBase
{
    private readonly IFilmeService _service;

    public FilmesController(IFilmeService service)
    {
        _service = service;
    }

    // GET: api/filmes
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FilmeResumoDto>>> GetFilmes()
    {
        var filmes = await _service.GetAllFilmesAsync();
        return Ok(filmes);
    }

    // GET: api/filmes/pesquisar-tmdb?query=Matrix
    [HttpGet("pesquisar-tmdb")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FilmeResumoDto>>> SearchTmdb([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("O termo de pesquisa não pode estar vazio.");

        var resultados = await _service.SearchFilmesTmdbAsync(query);
        return Ok(resultados);
    }

    // POST: api/filmes/importar/603
    [HttpPost("importar/{tmdbId}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<FilmeDetalheDto>> Import(int tmdbId)
    {
        try
        {
            var resultado = await _service.ImportFilmeFromTmdbAsync(tmdbId);

            return CreatedAtAction(nameof(GetFilmes), new { id = resultado.Id }, resultado);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
