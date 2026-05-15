using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/festivals")]
public class FestivalsController : ControllerBase
{
    private readonly IFestivalService _service;

    public FestivalsController(IFestivalService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FestivalResumoDto>>> GetAll()
    {
        var festivals = await _service.GetAllAsync();

        return Ok(festivals);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<FestivalDetalheDto>> GetById(int id)
    {
        var festival = await _service.GetByIdAsync(id);

        if (festival == null)
            return NotFound("Festival não encontrado.");

        return Ok(festival);
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<FestivalDetalheDto>> Create(CriarFestivalDto dto)
    {
        try
        {
            var festival = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = festival.Id }, festival);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Update(int id, AtualizarFestivalDto dto)
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
