using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/rewards")]
public class RewardsController : ControllerBase
{
    private readonly IRewardsQueryService _queryService;

    public RewardsController(IRewardsQueryService queryService)
    {
        _queryService = queryService;
    }

    [HttpGet("{utilizadorId}")]
    public IActionResult GetSaldo(string utilizadorId)
    {
        var saldo = _queryService.GetSaldo(utilizadorId);
        return Ok(new { utilizadorId, pontos = saldo });
    }

    [HttpGet("{utilizadorId}/historico")]
    public IActionResult GetHistorico(string utilizadorId)
    {
        var historico = _queryService.GetHistorico(utilizadorId)
            .Select(t => new RewardTransacaoReadDto
            {
                Id = t.Id,
                UtilizadorId = t.UtilizadorId,
                Pontos = t.Pontos,
                Data = t.Data,
                Motivo = t.Motivo
            });

        return Ok(historico);
    }
}
