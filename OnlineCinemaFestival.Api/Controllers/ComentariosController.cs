using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ComentariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ComentariosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Comentarios/filme/{filmeId}
    [HttpGet("filme/{filmeId}")]
    public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetComentariosPorFilme(int filmeId)
    {
        /*if (!await _context.Filmes.AnyAsync(f => f.Id == filmeId))
        {
            return NotFound($"Filme com ID {filmeId} não encontrado.");
        }*/

        var comentarios = await _context.Comentarios
            .AsNoTracking()
            .Where(c => c.FilmeId == filmeId && c.Visivel == true)
            .OrderByDescending(c => c.Data)
            .Select(c => new ComentarioReadDto
            {
                Id = c.Id,
                FilmeId = c.FilmeId,
                UsuarioId = c.UsuarioId,
                Texto = c.Texto,
                Data = c.Data
            })
            .ToListAsync();

        return Ok(comentarios);
    }

    // GET: api/Comentarios/{id} -> para obter um comentario especifico
    [HttpGet("{id}")]
    public async Task<ActionResult<ComentarioReadDto>> GetById(int id)
    {
        var comentario = await _context.Comentarios
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comentario == null)
        {
            return NotFound($"Comentário com ID {id} não encontrado.");
        }

        var readDto = new ComentarioReadDto
        {
            Id = comentario.Id,
            FilmeId = comentario.FilmeId,
            UsuarioId = comentario.UsuarioId,
            Texto = comentario.Texto,
            Data = comentario.Data
        };

        return Ok(readDto);
    }

    // POST: api/Comentarios
    [HttpPost]
    public async Task<ActionResult<ComentarioReadDto>> Post([FromBody] ComentarioCreateDto comentarioDto)
    {
        if (comentarioDto == null || string.IsNullOrWhiteSpace(comentarioDto.Texto))
        {
            return BadRequest("O texto do comentário é obrigatório.");
        }

        var comentario = new Comentario
        {
            FilmeId = comentarioDto.FilmeId,
            UsuarioId = comentarioDto.UsuarioId,
            Texto = comentarioDto.Texto,
            Data = DateTime.Now,
            Reportado = false,
            Visivel = true
        };

        _context.Comentarios.Add(comentario);
        await _context.SaveChangesAsync();

        var readDto = new ComentarioReadDto
        {
            Id = comentario.Id,
            FilmeId = comentario.FilmeId,
            UsuarioId = comentario.UsuarioId,
            Texto = comentario.Texto,
            Data = comentario.Data
        };
        return CreatedAtAction(nameof(GetById), new { id = comentario.Id }, readDto);

    }

}

    
    
