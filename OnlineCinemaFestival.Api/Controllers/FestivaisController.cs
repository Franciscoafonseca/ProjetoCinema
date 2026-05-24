using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Common.Auth;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/festivals")]
[Route("api/festivais")]
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
        var festival = await _service.CriarAsync(dto);

        return CreatedAtAction(nameof(ObterPorId), new { id = festival.Id }, festival);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Atualizar(int id, AtualizarFestivalDTO dto)
    {
        await _service.AtualizarAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _service.EliminarAsync(id);

        return NoContent();
    }
}
