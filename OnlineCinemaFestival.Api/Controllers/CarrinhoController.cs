using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarrinhoController : ControllerBase
{
    private readonly ICarrinhoService _service;

    public CarrinhoController(ICarrinhoService service)
    {
        _service = service;
    }

    [HttpGet("{utilizadorId}")]
    public ActionResult<IEnumerable<CompraItemDto>> Get(string utilizadorId)
    {
        var itens = _service.ObterItens(utilizadorId);
        return Ok(itens);
    }

    [HttpPost("{utilizadorId}/items")]
    public IActionResult AddItem(string utilizadorId, [FromBody] CompraItemDto item)
    {
        _service.AdicionarItem(utilizadorId, item);
        return NoContent();
    }

    [HttpDelete("{utilizadorId}/items/{filmeId:int}")]
    public IActionResult RemoveItem(string utilizadorId, int filmeId)
    {
        var removed = _service.RemoverItem(utilizadorId, filmeId);
        return removed ? NoContent() : NotFound();
    }

    [HttpDelete("{utilizadorId}")]
    public IActionResult Clear(string utilizadorId)
    {
        _service.Limpar(utilizadorId);
        return NoContent();
    }
}
