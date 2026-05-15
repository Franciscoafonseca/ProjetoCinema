using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Extensions;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/comunidades/{comunidadeId}/[controller]")]
public class ComentariosController : ControllerBase
{
    private readonly IComentarioService _comentarioService;

    public ComentariosController(IComentarioService comentarioService)
    {
        _comentarioService = comentarioService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ComentarioDto>> CriarComentario(
        int comunidadeId,
        [FromBody] CriarComentarioDto dto
    )
    {
        try
        {
            var resultado = await _comentarioService.CriarComentarioAsync(
                comunidadeId,
                dto,
                User.GetUserId()
            );

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComentarioDto>>> ObterComentarios(
        int comunidadeId
    )
    {
        var comentarios = await _comentarioService.ObterComentariosPorComunidadeIdAsync(
            comunidadeId
        );
        return Ok(comentarios);
    }
}
