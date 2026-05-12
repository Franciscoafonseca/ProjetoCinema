using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Extensions;

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
    public async Task<ActionResult<ComentarioReadDto>> CriarComentario(int comunidadeId, [FromBody] ComentarioCreateDto dto)
    {
        try
        {
            var resultado = await _comentarioService.CriarComentarioAsync(comunidadeId, dto, User.GetUserId());
            
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
    

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> ObterComentarios(int comunidadeId)
    {
        var comentarios = await _comentarioService.ObterComentariosPorComunidadeIdAsync(comunidadeId);
        return Ok(comentarios);
    }

}
