using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/sessoes")]
public class SessoesController : ControllerBase
{
    private readonly ISessaoService _service;

    public SessoesController(ISessaoService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SessaoResumoDto>>> GetAll()
    {
        var sessoes = await _service.GetAllAsync();

        return Ok(sessoes);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<SessaoDetalheDto>> GetById(int id)
    {
        var sessao = await _service.GetByIdAsync(id);

        if (sessao == null)
            return NotFound("Sessão não encontrada.");

        return Ok(sessao);
    }

    [HttpGet("disponiveis")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SessaoResumoDto>>> GetDisponiveis()
    {
        var sessoes = await _service.GetDisponiveisAsync();

        return Ok(sessoes);
    }

    [HttpGet("{id:int}/estado")]
    [AllowAnonymous]
    public async Task<ActionResult<SessaoEstadoReadDto>> GetEstado(int id)
    {
        try
        {
            var estado = await _service.GetEstadoAsync(id);

            return Ok(estado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("festival/{festivalId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SessaoResumoDto>>> GetByFestival(int festivalId)
    {
        try
        {
            var sessoes = await _service.GetByFestivalIdAsync(festivalId);

            return Ok(sessoes);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("filme/{filmeId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SessaoResumoDto>>> GetByFilme(int filmeId)
    {
        try
        {
            var sessoes = await _service.GetByFilmeIdAsync(filmeId);

            return Ok(sessoes);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<SessaoDetalheDto>> Create(CriarSessaoDto dto)
    {
        try
        {
            var sessao = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = sessao.Id }, sessao);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Update(int id, SessaoUpdateDto dto)
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
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
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
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
