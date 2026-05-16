using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/profiles")]
public class PerfisController : ControllerBase
{
    private readonly IPerfilUtilizadorService _profileService;

    public PerfisController(IPerfilUtilizadorService profileService)
    {
        _profileService = profileService;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<PerfilPrivadoDTO>> GetMyProfile()
    {
        var userId = ObterUtilizadorAtualId();

        if (userId == null)
            return Unauthorized();

        return Ok(await _profileService.ObterMeuPerfilAsync(userId.Value));
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<PerfilPrivadoDTO>> UpdateMyProfile(
        PedidoAtualizarPerfilDTO request
    )
    {
        var userId = ObterUtilizadorAtualId();

        if (userId == null)
            return Unauthorized();

        return Ok(await _profileService.AtualizarMeuPerfilAsync(userId.Value, request));
    }

    [Authorize]
    [HttpPost("me/foto")]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public async Task<ActionResult<PerfilPrivadoDTO>> UploadFoto(IFormFile ficheiro)
    {
        var userId = ObterUtilizadorAtualId();

        if (userId == null)
            return Unauthorized();

        try
        {
            return Ok(await _profileService.EnviarFotoPerfilAsync(userId.Value, ficheiro));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("public")]
    public async Task<ActionResult<List<PerfilPublicoDTO>>> GetPublicProfiles()
    {
        return Ok(await _profileService.ObterPerfisPublicosAsync());
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<PerfilPublicoDTO>> GetPublicProfile(int userId)
    {
        try
        {
            return Ok(await _profileService.ObterPerfilPublicoAsync(userId));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    private int? ObterUtilizadorAtualId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(value, out var userId))
            return userId;

        return null;
    }
}
