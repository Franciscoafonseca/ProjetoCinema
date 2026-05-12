using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;
using Microsoft.AspNetCore.Authorization;
using OnlineCinemaFestival.Api.Extensions;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ComunidadesController : ControllerBase
{
    private readonly IComunidadeService _comunidadeService;

    public ComunidadesController(IComunidadeService comunidadeService)
    {
        _comunidadeService = comunidadeService;
    }

    // Para apresentar todas as comunidades
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComunidadeReadDto>>> GetAll()
    {
        var comunidades = await _comunidadeService.GetAllComunidadesAsync(User.GetUserId());
        return Ok(comunidades);
    }

    [HttpGet("minhas")]
    public async Task<ActionResult<IEnumerable<ComunidadeReadDto>>> GetMinhasComunidades()
    {
        var comunidades = await _comunidadeService.GetMinhasComunidadesAsync(User.GetUserId());
        return Ok(comunidades);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ComunidadeReadDto>> GetComunidadeById(int id)
    {
        try
        {
            var comunidade = await _comunidadeService.GetComunidadeByIdAsync(id, User.GetUserId());
            if (comunidade == null) return NotFound("Comunidade não encontrada.");
            return Ok(comunidade);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { mensagem = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ComunidadeReadDto>> CreateComunidade(ComunidadeCreateDto dto)
    {
        try
        {
            var comunidadeCriada = await _comunidadeService.CreateComunidadeAsync(dto, User.GetUserId());
            return CreatedAtAction(nameof(GetComunidadeById), new { id = comunidadeCriada.Id }, comunidadeCriada);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }



}

