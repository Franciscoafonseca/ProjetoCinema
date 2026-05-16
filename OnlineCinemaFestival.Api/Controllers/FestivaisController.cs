using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/festivals")]
public class FestivaisController : ControllerBase
{
    private readonly IFestivalService _service;

    public FestivaisController(IFestivalService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FestivalResumoDTO>>> ObterTodos()
    {
        var festivals = await _service.ObterTodosAsync();

        return Ok(festivals);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<FestivalDetalheDTO>> ObterPorId(int id)
    {
        var festival = await _service.ObterPorIdAsync(id);

        if (festival == null)
            return NotFound("Festival não encontrado.");

        return Ok(festival);
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<FestivalDetalheDTO>> Criar(CriarFestivalDTO dto)
    {
        try
        {
            var festival = await _service.CriarAsync(dto);

            return CreatedAtAction(nameof(ObterPorId), new { id = festival.Id }, festival);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Atualizar(int id, AtualizarFestivalDTO dto)
    {
        try
        {
            await _service.AtualizarAsync(id, dto);

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
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _service.EliminarAsync(id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
