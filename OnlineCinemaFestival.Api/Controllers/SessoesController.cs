using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Common.Auth;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Extensions;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/sessoes")]
public class SessoesController : ControllerBase
{
    private readonly ISessaoService _service;
    private readonly IChatSessaoService _chatSessaoService;

    public SessoesController(ISessaoService service, IChatSessaoService chatSessaoService)
    {
        _service = service;
        _chatSessaoService = chatSessaoService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SessaoResumoDTO>>> ObterTodos()
    {
        var sessoes = await _service.ObterTodosAsync();

        return Ok(sessoes);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<SessaoDetalheDTO>> ObterPorId(int id)
    {
        var sessao = await _service.ObterPorIdAsync(id);

        if (sessao == null)
            return NotFound("Sessão não encontrada.");

        return Ok(sessao);
    }

    [HttpGet("disponiveis")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SessaoResumoDTO>>> GetDisponiveis()
    {
        var sessoes = await _service.ObterDisponiveisAsync();

        return Ok(sessoes);
    }

    [HttpGet("{id:int}/estado")]
    [AllowAnonymous]
    public async Task<ActionResult<SessaoEstadoReadDTO>> GetEstado(int id)
    {
        var estado = await _service.ObterEstadoAsync(id);

        return Ok(estado);
    }

    [HttpGet("{id:int}/chat/mensagens")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<IEnumerable<MensagemChatSessaoReadDTO>>> ObterMensagensChat(
        int id,
        [FromQuery] int quantidade = 50
    )
    {
        var mensagens = await _chatSessaoService.ObterHistoricoRecenteAsync(
            id,
            User.GetUserId(),
            User.IsInRole(NomesPapeis.Administrador),
            quantidade
        );

        return Ok(mensagens);
    }

    [HttpGet("festival/{festivalId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SessaoResumoDTO>>> GetByFestival(int festivalId)
    {
        var sessoes = await _service.ObterPorFestivalIdAsync(festivalId);

        return Ok(sessoes);
    }

    [HttpGet("filme/{filmeId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SessaoResumoDTO>>> GetByFilme(int filmeId)
    {
        var sessoes = await _service.ObterPorFilmeIdAsync(filmeId);

        return Ok(sessoes);
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<SessaoDetalheDTO>> Criar(CriarSessaoDTO dto)
    {
        var sessao = await _service.CriarAsync(dto);

        return CreatedAtAction(nameof(ObterPorId), new { id = sessao.Id }, sessao);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Atualizar(int id, SessaoUpdateDTO dto)
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
