using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/rewards")]
[Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
public class RewardsController : ControllerBase
{
    private readonly IRewardsQueryService _queryService;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public RewardsController(
        IRewardsQueryService queryService,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _queryService = queryService;
        _utilizadorAtualService = utilizadorAtualService;
    }

    [HttpGet]
    public IActionResult ObterSaldo()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();
        var saldo = _queryService.ObterSaldo(utilizadorId);

        return Ok(saldo);
    }

    [HttpGet("historico")]
    public IActionResult ObterHistorico()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();
        var historico = _queryService.ObterHistorico(utilizadorId);

        return Ok(historico);
    }
}
