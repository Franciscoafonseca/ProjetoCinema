using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Common.Auth;
using OnlineCinemaFestival.Api.Application.DTOs;
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
    public async Task<ActionResult<CarrinhoDTO>> ObterCarrinho()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();
        var carrinho = await _carrinhoService.ObterCarrinhoAsync(utilizadorId);

        return Ok(carrinho);
    }

    [HttpPost("itens")]
    public async Task<ActionResult<CarrinhoDTO>> AdicionarItem(AdicionarItemCarrinhoDTO dto)
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var carrinho = await _carrinhoService.AdicionarItemAsync(utilizadorId, dto);

        return Ok(carrinho);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CarrinhoDTO>> AdicionarItemPorTipo(CarrinhoItemCreateDTO dto)
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var carrinho = await _carrinhoService.AdicionarItemAsync(utilizadorId, dto);

        return Ok(carrinho);
    }

    [HttpPut("items/{itemId:int}")]
    [HttpPut("itens/{itemId:int}")]
    public async Task<ActionResult<CarrinhoDTO>> AtualizarItem(
        int itemId,
        CarrinhoItemUpdateDTO dto
    )
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var carrinho = await _carrinhoService.AtualizarItemAsync(utilizadorId, itemId, dto);

        return Ok(carrinho);
    }

    [HttpDelete("itens/{itemId:int}")]
    [HttpDelete("items/{itemId:int}")]
    public async Task<IActionResult> RemoverItem(int itemId)
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        await _carrinhoService.RemoverItemAsync(utilizadorId, itemId);

        return NoContent();
    }

    [HttpDelete]
    [HttpDelete("limpar")]
    public async Task<IActionResult> LimparCarrinho()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        await _carrinhoService.LimparCarrinhoAsync(utilizadorId);

        return NoContent();
    }

    [HttpPost("validar")]
    public async Task<ActionResult<CarrinhoValidacaoDTO>> ValidarCarrinho()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();
        var resultado = await _carrinhoService.ValidarCarrinhoAsync(utilizadorId);

        return Ok(resultado);
    }

    [HttpGet("resumo")]
    public async Task<ActionResult<CarrinhoResumoDTO>> ObterResumo()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();
        var resumo = await _carrinhoService.ObterResumoAsync(utilizadorId);

        return Ok(resumo);
    }
}
