using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/acessos")]
public class AcessosController : ControllerBase
{
    private readonly IAcessoService _service;

    public AcessosController(IAcessoService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AcessoReadDto>>> GetAll()
    {
        var acessos = await _service.GetAllAsync();

        return Ok(acessos);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<AcessoReadDto>> GetById(int id)
    {
        var acesso = await _service.GetByIdAsync(id);

        if (acesso == null)
            return NotFound("Acesso não encontrado.");

        return Ok(acesso);
    }

    [HttpGet("tipos")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<TipoAcessoReadDto>> GetTipos()
    {
        var tipos = _service.GetTiposAcesso();

        return Ok(tipos);
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<AcessoReadDto>> Create(AcessoCreateDto dto)
    {
        try
        {
            var acesso = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = acesso.Id }, acesso);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Update(int id, AcessoUpdateDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
