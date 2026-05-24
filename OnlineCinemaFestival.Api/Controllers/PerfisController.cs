using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Extensions;
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
        return Ok(await _profileService.ObterMeuPerfilAsync(User.GetUserId()));
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<PerfilPrivadoDTO>> UpdateMyProfile(
        PedidoAtualizarPerfilDTO request
    )
    {
        return Ok(await _profileService.AtualizarMeuPerfilAsync(User.GetUserId(), request));
    }

    [Authorize]
    [HttpPost("foto")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PerfilPrivadoDTO>> UploadFoto([FromForm] IFormFile foto)
    {
        return Ok(await _profileService.EnviarFotoPerfilAsync(User.GetUserId(), foto));
    }

    [HttpGet("public")]
    public async Task<ActionResult<List<PerfilPublicoDTO>>> GetPublicProfiles()
    {
        return Ok(await _profileService.ObterPerfisPublicosAsync());
    }

    [AllowAnonymous]
    [HttpGet("opcoes")]
    public ActionResult<PerfilOpcoesDTO> GetOpcoesPerfil()
    {
        return Ok(
            new PerfilOpcoesDTO
            {
                Paises = PerfilOpcoes
                    .Paises.Select(p => new PaisOpcaoDTO { Codigo = p.Codigo, Nome = p.Nome })
                    .ToList(),
                Localidades = PerfilOpcoes.Localidades.ToList(),
            }
        );
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<PerfilPublicoDTO>> GetPublicProfile(int userId)
    {
        return Ok(await _profileService.ObterPerfilPublicoAsync(userId));
    }
}
