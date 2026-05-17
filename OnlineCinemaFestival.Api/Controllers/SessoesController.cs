using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
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
        try
        {
            var estado = await _service.ObterEstadoAsync(id);

            return Ok(estado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id:int}/chat/mensagens")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<IEnumerable<MensagemChatSessaoReadDTO>>> ObterMensagensChat(
        int id,
        [FromQuery] int quantidade = 50
    )
    {
        try
        {
            var mensagens = await _chatSessaoService.ObterHistoricoRecenteAsync(
                id,
                User.GetUserId(),
                User.IsInRole(NomesPapeis.Administrador),
                quantidade
            );

            return Ok(mensagens);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("festival/{festivalId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SessaoResumoDTO>>> GetByFestival(int festivalId)
    {
        try
        {
            var sessoes = await _service.ObterPorFestivalIdAsync(festivalId);

            return Ok(sessoes);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("filme/{filmeId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SessaoResumoDTO>>> GetByFilme(int filmeId)
    {
        try
        {
            var sessoes = await _service.ObterPorFilmeIdAsync(filmeId);

            return Ok(sessoes);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<SessaoDetalheDTO>> Criar(CriarSessaoDTO dto)
    {
        try
        {
            var sessao = await _service.CriarAsync(dto);

            return CreatedAtAction(nameof(ObterPorId), new { id = sessao.Id }, sessao);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Atualizar(int id, SessaoUpdateDTO dto)
    {
        try
        {
            await _service.AtualizarAsync(id, dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:int}/filmes")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<SessaoDetalheDTO>> AssociarFilme(
        int id,
        AssociarFilmeSessaoDTO dto
    )
    {
        try
        {
            var sessao = await _service.AssociarFilmeAsync(id, dto);

            return Ok(sessao);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _service.EliminarAsync(id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
