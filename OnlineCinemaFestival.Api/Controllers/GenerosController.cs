using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/genres")]
public class GenerosController : ControllerBase
{
    private readonly IGeneroService _generoService;

    public GenerosController(IGeneroService generoService)
    {
        _generoService = generoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GeneroDTO>>> ObterTodos()
    {
        var generos = await _generoService.ObterTodosAsync();

        return Ok(generos);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<GeneroDTO>> Criar(CriarGeneroDTO dto)
    {
        try
        {
            var criado = await _generoService.CriarAsync(dto);

            return CreatedAtAction(nameof(ObterTodos), new { id = criado.Id }, criado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
