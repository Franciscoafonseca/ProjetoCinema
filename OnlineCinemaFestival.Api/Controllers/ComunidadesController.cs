using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Application.DTOs;
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
        var comunidade = await _comunidadeService.ObterComunidadePorPublicIdAsync(
            id,
            User.GetUserId()
        );

        if (comunidade == null)
            return NotFound("Comunidade nao encontrada.");

        return Ok(comunidade);
    }

    [HttpPost]
    public async Task<ActionResult<ComunidadeReadDTO>> CriarComunidade(ComunidadeCreateDTO dto)
    {
        var comunidadeCriada = await _comunidadeService.CriarComunidadeAsync(dto, User.GetUserId());

        return CreatedAtAction(
            nameof(ObterComunidadePorId),
            new { id = comunidadeCriada.PublicId },
            comunidadeCriada
        );
    }

    [HttpPost("{id:guid}/imagem")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ComunidadeReadDTO>> UploadImagem(Guid id, [FromForm] IFormFile imagem)
    {
        return Ok(await _comunidadeService.EnviarImagemAsync(id, User.GetUserId(), imagem));
    }

    [HttpGet("convite/{codigoConvite}")]
    public async Task<ActionResult<ComunidadeReadDTO>> ObterComunidadePorConvite(
        string codigoConvite
    )
    {
        var comunidade = await _comunidadeService.ObterComunidadePorConviteAsync(
            codigoConvite,
            User.GetUserId()
        );
        if (comunidade == null)
            return NotFound("Comunidade nao encontrada.");

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

    // Para entrar com o Código de Convite
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

    [HttpPost("{id:guid}/sair")]
    public async Task<ActionResult> SairComunidade(Guid id)
    {
        try
        {
            await _comunidadeService.SairComunidadeAsync(id, User.GetUserId());
            return Ok(new { mensagem = "Saíste da comunidade com sucesso!" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
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

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> ApagarComunidade(Guid id)
    {
        try
        {
            await _comunidadeService.ApagarComunidadeAsync(id, User.GetUserId());
            return Ok(new { mensagem = "Comunidade apagada com sucesso!" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
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
