using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController : ControllerBase
{
    private readonly IGeneroRepository _generoRepository;
    private readonly AppDbContext _context;

    public GenresController(IGeneroRepository generoRepository, AppDbContext context)
    {
        _generoRepository = generoRepository;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Genero>>> GetAll()
    {
        return Ok(await _generoRepository.GetAllAsync());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Genero>> Create(Genero genero)
    {
        genero.Name = genero.Name.Trim();

        if (string.IsNullOrWhiteSpace(genero.Name))
            return BadRequest("O nome do género é obrigatório.");

        _context.Generos.Add(genero);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = genero.Id }, genero);
    }
}
