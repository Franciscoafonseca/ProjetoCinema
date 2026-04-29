using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<IEnumerable<FilmeReadDto>>> GetFilmes()
    {
        var filmes = await _service.GetAllFilmesAsync();
        return Ok(filmes);
    }

    // GET: api/filmes/pesquisar-tmdb?query=Matrix
    [HttpGet("pesquisar-tmdb")]
    public async Task<ActionResult<IEnumerable<FilmeReadDto>>> SearchTmdb([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("O termo de pesquisa não pode estar vazio.");

        var resultados = await _service.SearchFilmesTmdbAsync(query);
        return Ok(resultados);
    }

    // POST: api/filmes/importar/603
    [HttpPost("importar/{tmdbId}")]
    public async Task<ActionResult<FilmeReadDto>> Import(int tmdbId)
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