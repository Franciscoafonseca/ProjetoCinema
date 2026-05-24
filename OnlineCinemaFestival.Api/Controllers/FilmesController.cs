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
    private readonly IComentarioService _comentarioService;

    public FilmesController(
        IFilmeService service,
        IComentarioService comentarioService
    )
    {
        _service = service;
        _comentarioService = comentarioService;
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
        var resultado = await _service.ImportFilmeFromTmdbAsync(tmdbId);

        return CreatedAtAction(nameof(GetFilme), new { id = resultado.Id }, resultado);
    }

    [HttpPatch("{filmeId:int}/video")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<FilmeDetalheDTO>> AtualizarVideo(int filmeId)
    {
        return Ok(await _service.AtualizarVideoAsync(filmeId));
    }

    [HttpPost("{filmeId:int}/reviews")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<AvaliacaoDTO>> CriarReview(
        int filmeId,
        [FromBody] CriarAvaliacaoDTO dto
    )
    {
        var resultado = await _service.CriarReviewAsync(User.GetUserId(), filmeId, dto);
        return Ok(resultado);
    }

    [HttpPut("{filmeId:int}/reviews/minha")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<AvaliacaoDTO>> AtualizarMinhaReview(
        int filmeId,
        [FromBody] CriarAvaliacaoDTO dto
    )
    {
        var resultado = await _service.AtualizarReviewAsync(User.GetUserId(), filmeId, dto);
        return Ok(resultado);
    }

    [HttpGet("{filmeId:int}/comentarios")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ComentarioReadDTO>>> ObterComentarios(int filmeId)
    {
        var comentarios = await _comentarioService.ObterComentariosPorFilmeIdAsync(filmeId);
        return Ok(comentarios);
    }

    [HttpPost("{filmeId:int}/comentarios")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<ComentarioReadDTO>> CriarComentario(
        int filmeId,
        [FromBody] ComentarioCreateDTO dto
    )
    {
        var comentario = await _comentarioService.CriarComentarioFilmeAsync(
            filmeId,
            dto,
            User.GetUserId()
        );

        return Ok(comentario);
    }
}
