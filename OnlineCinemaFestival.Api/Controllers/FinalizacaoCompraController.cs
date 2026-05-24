using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Common.Auth;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/checkout")]
[Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
public class FinalizacaoCompraController : ControllerBase
{
    private readonly IFinalizacaoCompraService _checkoutService;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public FinalizacaoCompraController(
        IFinalizacaoCompraService FinalizacaoCompraService,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _checkoutService = FinalizacaoCompraService;
        _utilizadorAtualService = utilizadorAtualService;
    }

    [HttpPost]
    public async Task<ActionResult<ResultadoFinalizacaoCompraDTO>> FinalizarCompra(
        [FromBody] PedidoFinalizarCompraDTO request
    )
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var compra = await _checkoutService.FinalizarCompraAsync(utilizadorId, request.MetodoPagamento);

        return Ok(compra);
    }
}
