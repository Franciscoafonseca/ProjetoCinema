using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Extensions;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComunidadesController : ControllerBase
{
    private readonly IComunidadeService _comunidadeService;

    public ComunidadesController(IComunidadeService comunidadeService)
    {
        _comunidadeService = comunidadeService;
    }

    // Para apresentar todas as comunidades
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComunidadeReadDTO>>> ObterTodos()
    {
        var comunidades = await _comunidadeService.ObterTodasComunidadesAsync(User.GetUserId());
        return Ok(comunidades);
    }

    [HttpGet("minhas")]
    public async Task<ActionResult<IEnumerable<ComunidadeReadDTO>>> ObterMinhasComunidades()
    {
        var comunidades = await _comunidadeService.ObterMinhasComunidadesAsync(User.GetUserId());
        return Ok(comunidades);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ComunidadeReadDTO>> ObterComunidadePorId(Guid id)
    {
        try
        {
            var comunidade = await _comunidadeService.ObterComunidadePorPublicIdAsync(
                id,
                User.GetUserId()
            );
            if (comunidade == null)
                return NotFound("Comunidade não encontrada.");
            return Ok(comunidade);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { mensagem = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ComunidadeReadDTO>> CriarComunidade(ComunidadeCreateDTO dto)
    {
        try
        {
            var comunidadeCriada = await _comunidadeService.CriarComunidadeAsync(
                dto,
                User.GetUserId()
            );
            return CreatedAtAction(
                nameof(ObterComunidadePorId),
                new { id = comunidadeCriada.PublicId },
                comunidadeCriada
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("convite/{codigoConvite}")]
    public async Task<ActionResult<ComunidadeReadDTO>> ObterComunidadePorConvite(string codigoConvite)
    {
        var comunidade = await _comunidadeService.ObterComunidadePorConviteAsync(codigoConvite);
        if (comunidade == null)
            return NotFound("Comunidade não encontrada.");
        return Ok(comunidade);
    }

    [HttpPost("{id:guid}/aderir")]
    public async Task<ActionResult> AderirComunidade(Guid id)
    {
        try
        {
            await _comunidadeService.AderirComunidadeAsync(id, User.GetUserId());
            return Ok(new { mensagem = "Entraste na comunidade com sucesso!" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // 🎟️ Rota VIP para entrar com o Código de Convite
    [HttpPost("convite/{codigoConvite}/aderir")]
    public async Task<ActionResult> AderirPorConvite(string codigoConvite)
    {
        try
        {
            await _comunidadeService.AderirComunidadePorConviteAsync(
                codigoConvite,
                User.GetUserId()
            );
            return Ok(new { mensagem = "Convite aceite! Bem-vindo à comunidade." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
