using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
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
    public async Task<ActionResult<ComentarioReadDto>> CriarComentario(int comunidadeId, [FromBody] ComentarioCreateDto dto)
    {
        try
        {
            int usuarioId = 1; // TODO: Substituir pelo ID do usuário autenticado
            var resultado = await _comentarioService.CriarComentarioAsync(comunidadeId, dto, usuarioId);
            
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new {mensagem = ex.Message});
        }
    }
    

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> ObterComentarios(int comunidadeId)
    {
        var comentarios = await _comentarioService.ObterComentariosPorComunidadeIdAsync(comunidadeId);
        return Ok(comentarios);
    }

}
