using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Common.Auth;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/pagamentos-pendentes")]
[Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
public class PagamentosPendentesController : ControllerBase
{
    private readonly IPagamentosPendentesService _service;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public PagamentosPendentesController(
        IPagamentosPendentesService service,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _service = service;
        _utilizadorAtualService = utilizadorAtualService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CompraReadDTO>>> ObterPendentes()
    {
        return Ok(await _service.ObterDoUtilizadorAsync(_utilizadorAtualService.ObterUtilizadorId()));
    }

    [HttpPost("{compraId:int}/confirmar")]
    public async Task<ActionResult<CompraReadDTO>> Confirmar(int compraId)
    {
        return Ok(
            await _service.ConfirmarPagamentoAsync(
                compraId,
                _utilizadorAtualService.ObterUtilizadorId()
            )
        );
    }
}
