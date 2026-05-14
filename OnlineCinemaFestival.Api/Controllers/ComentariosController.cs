using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Extensions;

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
    public async Task<ActionResult<ComentarioReadDto>> CriarComentario(Guid comunidadeId, [FromBody] ComentarioCreateDto dto)
    {
        try
        {
            var resultado = await _comentarioService.CriarComentarioAsync(comunidadeId, dto, User.GetUserId());
            
            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
    

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> ObterComentarios(Guid comunidadeId)
    {
        try
        {
            var comentarios = await _comentarioService.ObterComentariosPorComunidadeIdAsync(comunidadeId, User.GetUserId());
            return Ok(comentarios);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

}
