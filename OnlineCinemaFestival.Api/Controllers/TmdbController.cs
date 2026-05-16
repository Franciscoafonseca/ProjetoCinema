using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/tmdb")]
[AllowAnonymous]
public class TmdbController : ControllerBase
{
    private readonly ITmdbService _tmdbService;

    public TmdbController(ITmdbService tmdbService)
    {
        _tmdbService = tmdbService;
    }

    [HttpGet("pesquisar")]
    public async Task<ActionResult<IEnumerable<TmdbFilmeDTO>>> Pesquisar([FromQuery] string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return BadRequest("O termo de pesquisa e obrigatorio.");

        return Ok(await _tmdbService.SearchFilmesTmdbAsync(termo));
    }

    [HttpGet("filmes-iniciais")]
    public async Task<ActionResult<IEnumerable<TmdbFilmeDTO>>> FilmesIniciais()
    {
        return Ok(await _tmdbService.ObterFilmesIniciaisAsync());
    }

    [HttpGet("generos")]
    public async Task<ActionResult<IEnumerable<TmdbGeneroDTO>>> Generos()
    {
        return Ok(await _tmdbService.ObterGenerosAsync());
    }
}
