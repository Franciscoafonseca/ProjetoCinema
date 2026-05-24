using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Extensions;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api")]
public class PremiosFestivalController : ControllerBase
{
    private readonly IPremioFestivalService _service;
    private readonly IPublicacaoPremiosService _publicacaoPremiosService;

    public PremiosFestivalController(
        IPremioFestivalService service,
        IPublicacaoPremiosService publicacaoPremiosService
    )
    {
        _service = service;
        _publicacaoPremiosService = publicacaoPremiosService;
    }

    [HttpGet("festivals/{festivalId:int}/premios")]
    [HttpGet("festivais/{festivalId:int}/premios")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PremioFestivalReadDTO>>> ObterPremios(
        int festivalId
    )
    {
        var incluirRascunhos = User.IsInRole(NomesPapeis.Administrador);
        var premios = await _service.ObterPremiosPorFestivalAsync(
            festivalId,
            incluirRascunhos
        );

        return Ok(premios);
    }

    [HttpGet("festivals/{festivalId:int}/premios/resultados-publicos")]
    [HttpGet("festivais/{festivalId:int}/premios/resultados-publicos")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ResultadoPremioFestivalDTO>>> ObterResultadosFestival(
        int festivalId
    )
    {
        var resultados = await _service.ObterResultadosPublicosAsync(festivalId: festivalId);
        return Ok(resultados);
    }

    [HttpGet("filmes/{filmeId:int}/premios-resultados")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ResultadoPremioFestivalDTO>>> ObterResultadosFilme(
        int filmeId
    )
    {
        var resultados = await _service.ObterResultadosPublicosAsync(filmeId: filmeId);
        return Ok(resultados);
    }

    [HttpPost("festivals/{festivalId:int}/premios")]
    [HttpPost("festivais/{festivalId:int}/premios")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<PremioFestivalReadDTO>> CriarPremio(
        int festivalId,
        CriarPremioFestivalDTO dto
    )
    {
        var premio = await _service.CriarPremioAsync(festivalId, dto);
        return CreatedAtAction(nameof(ObterPremios), new { festivalId }, premio);
    }

    [HttpPost("premios-festival/{premioFestivalId:int}/abrir")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<PremioFestivalReadDTO>> AbrirVotacao(int premioFestivalId)
    {
        return Ok(await _service.AbrirVotacaoAsync(premioFestivalId));
    }

    [HttpPost("premios-festival/{premioFestivalId:int}/votos")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<IActionResult> Votar(int premioFestivalId, VotarPremioFestivalDTO dto)
    {
        await _service.VotarAsync(premioFestivalId, dto.FilmeId, User.GetUserId());
        return NoContent();
    }

    [HttpPost("premios-festival/{premioFestivalId:int}/fechar")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<PremioFestivalReadDTO>> FecharVotacao(int premioFestivalId)
    {
        return Ok(await _service.FecharVotacaoAsync(premioFestivalId));
    }

    [HttpPost("premios-festival/{premioFestivalId:int}/publicar")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<ResultadoPremioFestivalDTO>> PublicarResultados(
        int premioFestivalId
    )
    {
        return Ok(await _service.PublicarResultadosAsync(premioFestivalId, User.GetUserId()));
    }

    [HttpPost("premios-festival/publicar-pendentes")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<int>> PublicarResultadosPendentes(
        CancellationToken cancellationToken
    )
    {
        var publicados = await _publicacaoPremiosService.PublicarResultadosPendentesAsync(
            cancellationToken
        );

        return Ok(publicados);
    }
}
