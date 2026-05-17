using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Extensions;
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
    public async Task<ActionResult<IEnumerable<FilmeResumoDTO>>> GetFilmes()
    {
        var filmes = await _service.ObterTodosFilmesAsync();
        return Ok(filmes);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<FilmeDetalheDTO>> GetFilme(int id)
    {
        var utilizadorId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (int?)null;
        var filme = await _service.ObterDetalheAsync(id, utilizadorId);

        if (filme == null)
            return NotFound("Filme nao encontrado.");

        return Ok(filme);
    }

    // GET: api/filmes/pesquisar-tmdb?query=Matrix
    [HttpGet("pesquisar-tmdb")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FilmeResumoDTO>>> PesquisarTmdb([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("O termo de pesquisa não pode estar vazio.");

        var resultados = await _service.SearchFilmesTmdbAsync(query);
        return Ok(resultados);
    }

    // POST: api/filmes/importar/603
    [HttpPost("importar/{tmdbId}")]
    [HttpPost("importar-tmdb/{tmdbId}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<FilmeDetalheDTO>> Importar(int tmdbId)
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

    [HttpPatch("{filmeId:int}/video")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<FilmeDetalheDTO>> AtualizarVideo(
        int filmeId,
        [FromBody] AtualizarVideoFilmeDTO dto
    )
    {
        try
        {
            return Ok(await _service.AtualizarVideoAsync(filmeId, dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{filmeId:int}/reviews")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<AvaliacaoDTO>> CriarReview(
        int filmeId,
        [FromBody] CriarAvaliacaoDTO dto
    )
    {
        try
        {
            var resultado = await _service.CriarReviewAsync(User.GetUserId(), filmeId, dto);
            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
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
