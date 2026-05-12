using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/visualizacao")]
[Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
public class VisualizacaoController : ControllerBase
{
    private readonly IVisualizacaoService _visualizacaoService;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public VisualizacaoController(
        IVisualizacaoService visualizacaoService,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _utilizadorAtualService = utilizadorAtualService;
        _visualizacaoService = visualizacaoService;
    }

    [HttpGet("filmes/{filmeId:int}")]
    public async Task<ActionResult<VisualizacaoReadDto>> VisualizarFilme(
        int filmeId,
        [FromQuery] int? festivalId
    )
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

            var visualizacao = await _visualizacaoService.ObterVisualizacaoFilmeAsync(
                utilizadorId,
                filmeId,
                festivalId
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
    }

    [HttpGet("sessoes/{sessaoId:int}")]
    public async Task<ActionResult<VisualizacaoReadDto>> VisualizarSessao(int sessaoId)
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

            var visualizacao = await _visualizacaoService.ObterVisualizacaoSessaoAsync(
                utilizadorId,
                sessaoId
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
    }
}
