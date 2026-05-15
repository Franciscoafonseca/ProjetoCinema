using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/carrinho")]
[Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
public class CarrinhoController : ControllerBase
{
    private readonly ICarrinhoService _carrinhoService;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public CarrinhoController(
        ICarrinhoService carrinhoService,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _carrinhoService = carrinhoService;
        _utilizadorAtualService = utilizadorAtualService;
    }

    [HttpGet]
    public async Task<ActionResult<CarrinhoReadDto>> ObterCarrinho()
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();
            var carrinho = await _carrinhoService.ObterCarrinhoAsync(utilizadorId);

            return Ok(carrinho);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("itens")]
    public async Task<ActionResult<CarrinhoReadDto>> AdicionarItem(AdicionarItemCarrinhoDto dto)
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

            var carrinho = await _carrinhoService.AdicionarItemAsync(utilizadorId, dto);

            return Ok(carrinho);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
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

    [HttpPost("items")]
    public async Task<ActionResult<CarrinhoReadDto>> AdicionarItemPorTipo(CarrinhoItemCreateDto dto)
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

            var carrinho = await _carrinhoService.AdicionarItemAsync(utilizadorId, dto);

            return Ok(carrinho);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
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

    [HttpPut("items/{itemId:int}")]
    [HttpPut("itens/{itemId:int}")]
    public async Task<ActionResult<CarrinhoReadDto>> AtualizarItem(
        int itemId,
        CarrinhoItemUpdateDto dto
    )
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

            var carrinho = await _carrinhoService.AtualizarItemAsync(utilizadorId, itemId, dto);

            return Ok(carrinho);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
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

    [HttpDelete("itens/{itemId:int}")]
    [HttpDelete("items/{itemId:int}")]
    public async Task<IActionResult> RemoverItem(int itemId)
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

            await _carrinhoService.RemoverItemAsync(utilizadorId, itemId);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete]
    [HttpDelete("limpar")]
    public async Task<IActionResult> LimparCarrinho()
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

            await _carrinhoService.LimparCarrinhoAsync(utilizadorId);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("validar")]
    public async Task<ActionResult<CarrinhoValidacaoDto>> ValidarCarrinho()
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();
            var resultado = await _carrinhoService.ValidarCarrinhoAsync(utilizadorId);

            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpGet("resumo")]
    public async Task<ActionResult<CarrinhoResumoDto>> ObterResumo()
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();
            var resumo = await _carrinhoService.ObterResumoAsync(utilizadorId);

            return Ok(resumo);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
