using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
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
        var saldo = _queryService.GetSaldo(utilizadorId);

        return Ok(new { utilizadorId, pontos = saldo });
    }

    [HttpGet("historico")]
    public IActionResult ObterHistorico()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();
        var historico = _queryService
            .GetHistorico(utilizadorId)
            .Select(t => new RewardTransacaoReadDto
            {
                Id = t.Id,
                UtilizadorId = t.UtilizadorId,
                Pontos = t.Pontos,
                Data = t.Data,
                Motivo = t.Motivo,
            });

        return Ok(historico);
    }
}
