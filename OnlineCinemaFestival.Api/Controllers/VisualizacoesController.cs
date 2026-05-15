using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/visualizacoes")]
[Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
public class VisualizacoesController : ControllerBase
{
    private readonly IVisualizacaoService _visualizacaoService;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public VisualizacoesController(
        IVisualizacaoService visualizacaoService,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _visualizacaoService = visualizacaoService;
        _utilizadorAtualService = utilizadorAtualService;
    }

    [HttpGet("minhas")]
    public async Task<ActionResult<IEnumerable<VisualizacaoHistoricoReadDto>>> ObterMinhas()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var visualizacoes = await _visualizacaoService.ObterHistoricoDoUtilizadorAsync(
            utilizadorId
        );

        return Ok(visualizacoes);
    }

    [HttpPost]
    public async Task<ActionResult<VisualizacaoHistoricoReadDto>> Registar(
        RegistarVisualizacaoDto dto
    )
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

            var visualizacao = await _visualizacaoService.RegistarVisualizacaoAsync(
                utilizadorId,
                dto
            );

            return Ok(visualizacao);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
