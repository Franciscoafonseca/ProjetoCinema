using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Common.Auth;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/acessos")]
public class AcessosController : ControllerBase
{
    private readonly IAcessoService _service;
    private readonly IAcessoUtilizadorService _acessoUtilizadorService;
    private readonly IUtilizadorAtualService _utilizadorAtualService;

    public AcessosController(
        IAcessoService service,
        IAcessoUtilizadorService acessoUtilizadorService,
        IUtilizadorAtualService utilizadorAtualService
    )
    {
        _service = service;
        _acessoUtilizadorService = acessoUtilizadorService;
        _utilizadorAtualService = utilizadorAtualService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AcessoDTO>>> ObterTodos()
    {
        var acessos = await _service.ObterTodosAsync();

        return Ok(acessos);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<AcessoDTO>> ObterPorId(int id)
    {
        var acesso = await _service.ObterPorIdAsync(id);

        if (acesso == null)
            return NotFound("Acesso não encontrado.");

        return Ok(acesso);
    }

    [HttpGet("tipos")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<TipoAcessoReadDTO>> ObterTipos()
    {
        var tipos = _service.GetTiposAcesso();

        return Ok(tipos);
    }

    [HttpGet("meus")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<IEnumerable<AcessoUtilizadorDTO>>> ObterMeusAcessos()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var acessos = await _acessoUtilizadorService.ObterAcessosAtivosDoUtilizadorAsync(
            utilizadorId
        );

        return Ok(acessos);
    }

    [HttpGet("validar-filme/{filmeId:int}")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<ValidacaoAcessoReadDTO>> ValidarFilme(
        int filmeId,
        [FromQuery] int? festivalId
    )
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var temAcesso = await _acessoUtilizadorService.UtilizadorTemAcessoAFilmeAsync(
            utilizadorId,
            filmeId,
            festivalId
        );

        return Ok(
            new ValidacaoAcessoReadDTO
            {
                TemAcesso = temAcesso,
                Mensagem = temAcesso ? "Acesso valido." : "Sem acesso valido para este filme.",
            }
        );
    }

    [HttpGet("validar-sessao/{sessaoId:int}")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<ValidacaoAcessoReadDTO>> ValidarSessao(int sessaoId)
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var temAcesso = await _acessoUtilizadorService.UtilizadorTemAcessoASessaoAsync(
            utilizadorId,
            sessaoId
        );

        return Ok(
            new ValidacaoAcessoReadDTO
            {
                TemAcesso = temAcesso,
                Mensagem = temAcesso ? "Acesso valido." : "Sem acesso valido para esta sessao.",
            }
        );
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<AcessoDTO>> Criar(CriarAcessoDTO dto)
    {
        var acesso = await _service.CriarAsync(dto);

        return CreatedAtAction(nameof(ObterPorId), new { id = acesso.Id }, acesso);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Atualizar(int id, AcessoUpdateDTO dto)
    {
        await _service.AtualizarAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _service.EliminarAsync(id);

        return NoContent();
    }
}
