using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Common.Auth;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/recomendacoes")]
[Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
public class RecomendacoesController : ControllerBase
{
    private readonly IRecomendacaoService _service;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public RecomendacoesController(
        IRecomendacaoService service,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _service = service;
        _utilizadorAtualService = utilizadorAtualService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FilmeRecomendadoDTO>>> Obter([FromQuery] int quantidade = 12)
    {
        return Ok(await _service.ObterRecomendacoesAsync(_utilizadorAtualService.ObterUtilizadorId(), quantidade));
    }
}
