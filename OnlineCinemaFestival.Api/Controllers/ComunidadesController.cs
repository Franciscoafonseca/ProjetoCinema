using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

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
        var comunidades = await _comunidadeService.GetAllComunidadesAsync();
        return Ok(comunidades);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ComunidadeReadDto>> GetComunidadeById(int id)
    {
        var comunidade = await _comunidadeService.GetComunidadeByIdAsync(id);
        if (comunidade == null) return NotFound("Comunidade não encontrada.");
        return Ok(comunidade);
    }

    [HttpPost]
    public async Task<ActionResult<ComunidadeReadDto>> CreateComunidade(ComunidadeCreateDto dto)
    {
        try
        {
            // mudar depois
            int criadorUserId = 1; // Simulação de um utilizador autenticado
            var comunidadeCriada = await _comunidadeService.CreateComunidadeAsync(dto, criadorUserId);
            return CreatedAtAction(nameof(GetComunidadeById), new { id = comunidadeCriada.Id }, comunidadeCriada);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }



}

