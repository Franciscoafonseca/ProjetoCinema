using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/checkout")]
[Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public CheckoutController(
        ICheckoutService checkoutService,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _checkoutService = checkoutService;
        _utilizadorAtualService = utilizadorAtualService;
    }

    [HttpPost]
    public async Task<ActionResult<CheckoutResultadoDto>> FinalizarCompra()
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

            var compra = await _checkoutService.FinalizarCompraAsync(utilizadorId);

            return Ok(compra);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
