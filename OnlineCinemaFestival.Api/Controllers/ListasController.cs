using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Extensions;
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
    public async Task<ActionResult<IEnumerable<ListaPessoalDTO>>> GetMinhasListas()
    {
        var listas = await _service.ObterMinhasListasAsync(User.GetUserId());
        return Ok(listas);
    }

    [HttpPost]
    public async Task<ActionResult<ListaPessoalDTO>> Criar(ListaPessoalCreateDTO dto)
    {
        var lista = await _service.CriarAsync(User.GetUserId(), dto);
        return CreatedAtAction(nameof(GetMinhasListas), new { id = lista.Id }, lista);
    }

    [HttpPost("{id:int}/filmes/{filmeId:int}")]
    public async Task<ActionResult<ListaPessoalItemReadDTO>> AdicionarFilme(int id, int filmeId)
    {
        var item = await _service.AdicionarFilmeAsync(User.GetUserId(), id, filmeId);
        return Ok(item);
    }

    [HttpDelete("{id:int}/filmes/{filmeId:int}")]
    public async Task<IActionResult> RemoverFilme(int id, int filmeId)
    {
        await _service.RemoverFilmeAsync(User.GetUserId(), id, filmeId);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> RemoverLista(int id)
    {
        await _service.RemoverListaAsync(User.GetUserId(), id);
        return NoContent();
    }
}
