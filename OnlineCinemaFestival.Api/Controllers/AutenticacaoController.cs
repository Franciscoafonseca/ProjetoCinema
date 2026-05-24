using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _authService;

    public AutenticacaoController(IAutenticacaoService AutenticacaoService)
    {
        _authService = AutenticacaoService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AutenticacaoRespostaDTO>> Registar(PedidoRegistoDTO request)
    {
        return Ok(await _authService.RegistarAsync(request));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AutenticacaoRespostaDTO>> Entrar(PedidoLoginDTO request)
    {
        return Ok(await _authService.EntrarAsync(request));
    }
}
