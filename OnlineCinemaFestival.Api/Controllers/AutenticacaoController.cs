using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _authService;
    private readonly IAutenticacaoExternaService _autenticacaoExternaService;

    public AutenticacaoController(
        IAutenticacaoService AutenticacaoService,
        IAutenticacaoExternaService autenticacaoExternaService
    )
    {
        _authService = AutenticacaoService;
        _autenticacaoExternaService = autenticacaoExternaService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AutenticacaoRespostaDTO>> Registar(PedidoRegistoDTO request)
    {
        try
        {
            return Ok(await _authService.RegistarAsync(request));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AutenticacaoRespostaDTO>> Entrar(PedidoLoginDTO request)
    {
        try
        {
            return Ok(await _authService.EntrarAsync(request));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("external/providers")]
    public async Task<ActionResult<List<ProvedorAutenticacaoExternaDTO>>> ObterProvedoresExternos()
    {
        return Ok(await _autenticacaoExternaService.ObterProvedoresAsync());
    }

    [HttpPost("external/login")]
    public async Task<ActionResult<AutenticacaoRespostaDTO>> EntrarExterno(
        PedidoAutenticacaoExternaDTO request
    )
    {
        try
        {
            return Ok(await _autenticacaoExternaService.AutenticarAsync(request));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (NotSupportedException ex)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, ex.Message);
        }
    }
}
