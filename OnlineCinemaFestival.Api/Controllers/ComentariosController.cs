using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Extensions;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/comunidades/{comunidadeId:guid}/comentarios")]
[Authorize]
public class ComentariosController : ControllerBase
{
    private readonly IComentarioService _comentarioService;

    public ComentariosController(IComentarioService comentarioService)
    {
        _comentarioService = comentarioService;
    }

    [HttpPost]
    public async Task<ActionResult<ComentarioReadDTO>> CriarComentario(
        Guid comunidadeId,
        [FromBody] ComentarioCreateDTO dto
    )
    {
        var resultado = await _comentarioService.CriarComentarioAsync(
            comunidadeId,
            dto,
            User.GetUserId()
        );

        return Ok(resultado);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComentarioReadDTO>>> ObterComentarios(
        Guid comunidadeId
    )
    {
        var comentarios = await _comentarioService.ObterComentariosPorComunidadeIdAsync(
            comunidadeId,
            User.GetUserId()
        );
        return Ok(comentarios);
    }

    [HttpPatch("{comentarioId:int}/moderacao")]
    public async Task<ActionResult<ComentarioReadDTO>> ModerarComentario(
        Guid comunidadeId,
        int comentarioId,
        ModerarComentarioDTO dto
    )
    {
        return Ok(
            await _comentarioService.ModerarComentarioComunidadeAsync(
                comunidadeId,
                comentarioId,
                dto,
                User.GetUserId()
            )
        );
    }
}
