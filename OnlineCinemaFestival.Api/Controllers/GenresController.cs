using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController : ControllerBase
{
    private readonly IGeneroService _generoService;

    public GenresController(IGeneroService generoService)
    {
        _generoService = generoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genero>>> GetAll()
    {
        var generos = await _generoService.GetAllAsync();

        return Ok(generos);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Genero>> Create(Genero genero)
    {
        try
        {
            var criado = await _generoService.CreateAsync(genero);

            return CreatedAtAction(nameof(GetAll), new { id = criado.Id }, criado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
