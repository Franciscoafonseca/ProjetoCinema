using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/listas")]
[Authorize]
public class ListasController : ControllerBase
{
    private readonly IListaPessoalService _service;

    public ListasController(IListaPessoalService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ListaPessoalDto>>> GetMinhasListas()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var listas = await _service.GetMinhasListasAsync(userId.Value);
        return Ok(listas);
    }

    [HttpPost]
    public async Task<ActionResult<ListaPessoalDto>> Create(ListaPessoalCreateDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var lista = await _service.CreateAsync(userId.Value, dto);
            return CreatedAtAction(nameof(GetMinhasListas), new { id = lista.Id }, lista);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:int}/filmes/{filmeId:int}")]
    public async Task<ActionResult<ListaPessoalItemReadDto>> AdicionarFilme(int id, int filmeId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            var item = await _service.AdicionarFilmeAsync(userId.Value, id, filmeId);
            return Ok(item);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id:int}/filmes/{filmeId:int}")]
    public async Task<IActionResult> RemoverFilme(int id, int filmeId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            await _service.RemoverFilmeAsync(userId.Value, id, filmeId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> RemoverLista(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        try
        {
            await _service.RemoverListaAsync(userId.Value, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private int? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(value, out var userId))
            return userId;

        return null;
    }
}
