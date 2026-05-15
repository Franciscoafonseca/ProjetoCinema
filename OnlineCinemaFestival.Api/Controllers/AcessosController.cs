using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.DTOs;
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
    public async Task<ActionResult<IEnumerable<AcessoReadDto>>> GetAll()
    {
        var acessos = await _service.GetAllAsync();

        return Ok(acessos);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<AcessoReadDto>> GetById(int id)
    {
        var acesso = await _service.GetByIdAsync(id);

        if (acesso == null)
            return NotFound("Acesso não encontrado.");

        return Ok(acesso);
    }

    [HttpGet("tipos")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<TipoAcessoReadDto>> GetTipos()
    {
        var tipos = _service.GetTiposAcesso();

        return Ok(tipos);
    }

    [HttpGet("meus")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<IEnumerable<AcessoUtilizadorReadDto>>> ObterMeusAcessos()
    {
        var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

        var acessos = await _acessoUtilizadorService.ObterAcessosAtivosDoUtilizadorAsync(
            utilizadorId
        );

        return Ok(acessos);
    }

    [HttpGet("validar-filme/{filmeId:int}")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<ValidacaoAcessoReadDto>> ValidarFilme(
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
            new ValidacaoAcessoReadDto
            {
                TemAcesso = temAcesso,
                Mensagem = temAcesso ? "Acesso valido." : "Sem acesso valido para este filme.",
            }
        );
    }

    [HttpGet("validar-sessao/{sessaoId:int}")]
    [Authorize(Policy = NomesPoliticas.UtilizadorAutenticado)]
    public async Task<ActionResult<ValidacaoAcessoReadDto>> ValidarSessao(int sessaoId)
    {
        try
        {
            var utilizadorId = _utilizadorAtualService.ObterUtilizadorId();

            var temAcesso = await _acessoUtilizadorService.UtilizadorTemAcessoASessaoAsync(
                utilizadorId,
                sessaoId
            );

            return Ok(
                new ValidacaoAcessoReadDto
                {
                    TemAcesso = temAcesso,
                    Mensagem = temAcesso ? "Acesso valido." : "Sem acesso valido para esta sessao.",
                }
            );
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<ActionResult<AcessoReadDto>> Create(AcessoCreateDto dto)
    {
        try
        {
            var acesso = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = acesso.Id }, acesso);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Update(int id, AcessoUpdateDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
