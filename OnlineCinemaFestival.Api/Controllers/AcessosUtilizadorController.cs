using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/acessos-utilizador")]
[Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
public class AcessosUtilizadorController : ControllerBase
{
    private readonly IAcessoUtilizadorService _acessoUtilizadorService;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public AcessosUtilizadorController(
        IAcessoUtilizadorService acessoUtilizadorService,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _acessoUtilizadorService = acessoUtilizadorService;
        _utilizadorAtualService = utilizadorAtualService;
    }

    [HttpGet("meus")]
    public async Task<ActionResult<IEnumerable<AcessoUtilizadorDTO>>> ObterMeusAcessos()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var acessos = await _acessoUtilizadorService.ObterAcessosDoUtilizadorAsync(utilizadorId);

        return Ok(acessos);
    }
}
