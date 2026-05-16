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
    public async Task<ActionResult<IEnumerable<TmdbFilmeDto>>> Pesquisar([FromQuery] string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return BadRequest("O termo de pesquisa e obrigatorio.");

        return Ok(await _tmdbService.SearchFilmesTmdbAsync(termo));
    }

    [HttpGet("filmes-iniciais")]
    public async Task<ActionResult<IEnumerable<TmdbFilmeDto>>> FilmesIniciais()
    {
        return Ok(await _tmdbService.GetFilmesIniciaisAsync());
    }

    [HttpGet("generos")]
    public async Task<ActionResult<IEnumerable<TmdbGeneroDto>>> Generos()
    {
        return Ok(await _tmdbService.GetGenerosAsync());
    }
}
