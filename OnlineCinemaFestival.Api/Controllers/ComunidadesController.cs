using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComunidadesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ComunidadesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var comunidades = await _db
            .Comunidades.Include(c => c.CreatedByUser)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.ImageUrl,
                c.IsPublic,
                c.CreatedByUserId,
                CreatedByUserName = c.CreatedByUser != null ? c.CreatedByUser.Name : null,
                c.CreatedAt,
                MembersCount = c.Members.Count,
                ComentariosCount = c.Comentarios.Count,
            })
            .ToListAsync();

        return Ok(comunidades);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var comunidade = await _db
            .Comunidades.Include(c => c.CreatedByUser)
            .Include(c => c.Members)
            .Include(c => c.Comentarios)
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.ImageUrl,
                c.IsPublic,
                c.CreatedByUserId,
                CreatedByUserName = c.CreatedByUser != null ? c.CreatedByUser.Name : null,
                c.CreatedAt,
                MembersCount = c.Members.Count,
                ComentariosCount = c.Comentarios.Count,
            })
            .FirstOrDefaultAsync();

        if (comunidade == null)
            return NotFound("Comunidade não encontrada.");

        return Ok(comunidade);
    }
}
