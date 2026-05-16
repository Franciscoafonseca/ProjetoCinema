using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/compras")]
[Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
public class ComprasController : ControllerBase
{
    private readonly ICompraService _compraService;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public ComprasController(
        ICompraService compraService,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _compraService = compraService;
        _utilizadorAtualService = utilizadorAtualService;
    }

    [HttpGet("minhas")]
    public async Task<ActionResult<IEnumerable<CompraDTO>>> ObterMinhasCompras()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var compras = await _compraService.ObterComprasDoUtilizadorAsync(utilizadorId);

        return Ok(compras);
    }
}
