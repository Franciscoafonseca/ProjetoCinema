using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComentariosController : ControllerBase
{
    private readonly AppDbContext _db;

    public ComentariosController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetAll()
    {
        var comentarios = await _db
            .Comentarios.Include(c => c.Usuario)
            .Include(c => c.Comunidade)
            .Where(c => c.Visivel)
            .OrderByDescending(c => c.CriadoEm)
            .Select(c => new ComentarioReadDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                NomeUsuario = c.Usuario.Name,
                ComunidadeId = c.ComunidadeId,
                NomeComunidade = c.Comunidade.Name,
                Texto = c.Texto,
                CriadoEm = c.CriadoEm,
                Visivel = c.Visivel,
                Reportado = c.Reportado,
            })
            .ToListAsync();

        return Ok(comentarios);
    }

    [HttpGet("comunidade/{comunidadeId:int}")]
    public async Task<ActionResult<IEnumerable<ComentarioReadDto>>> GetByComunidade(
        int comunidadeId
    )
    {
        var comentarios = await _db
            .Comentarios.Include(c => c.Usuario)
            .Include(c => c.Comunidade)
            .Where(c => c.ComunidadeId == comunidadeId && c.Visivel)
            .OrderByDescending(c => c.CriadoEm)
            .Select(c => new ComentarioReadDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                NomeUsuario = c.Usuario.Name,
                ComunidadeId = c.ComunidadeId,
                NomeComunidade = c.Comunidade.Name,
                Texto = c.Texto,
                CriadoEm = c.CriadoEm,
                Visivel = c.Visivel,
                Reportado = c.Reportado,
            })
            .ToListAsync();

        return Ok(comentarios);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ComentarioReadDto>> GetById(int id)
    {
        var comentario = await _db
            .Comentarios.Include(c => c.Usuario)
            .Include(c => c.Comunidade)
            .Where(c => c.Id == id)
            .Select(c => new ComentarioReadDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                NomeUsuario = c.Usuario.Name,
                ComunidadeId = c.ComunidadeId,
                NomeComunidade = c.Comunidade.Name,
                Texto = c.Texto,
                CriadoEm = c.CriadoEm,
                Visivel = c.Visivel,
                Reportado = c.Reportado,
            })
            .FirstOrDefaultAsync();

        if (comentario == null)
            return NotFound("Comentário não encontrado.");

        return Ok(comentario);
    }

    [HttpPost]
    public async Task<ActionResult<ComentarioReadDto>> Create(ComentarioCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Texto))
            return BadRequest("O texto do comentário é obrigatório.");

        var utilizadorExiste = await _db.Utilizadores.AnyAsync(u => u.Id == dto.UsuarioId);

        if (!utilizadorExiste)
            return BadRequest("Utilizador não encontrado.");

        var comunidadeExiste = await _db.Comunidades.AnyAsync(c => c.Id == dto.ComunidadeId);

        if (!comunidadeExiste)
            return BadRequest("Comunidade não encontrada.");

        var comentario = new Comentario
        {
            UsuarioId = dto.UsuarioId,
            ComunidadeId = dto.ComunidadeId,
            Texto = dto.Texto,
            CriadoEm = DateTime.UtcNow,
            Visivel = true,
            Reportado = false,
        };

        _db.Comentarios.Add(comentario);
        await _db.SaveChangesAsync();

        var criado = await _db
            .Comentarios.Include(c => c.Usuario)
            .Include(c => c.Comunidade)
            .Where(c => c.Id == comentario.Id)
            .Select(c => new ComentarioReadDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                NomeUsuario = c.Usuario.Name,
                ComunidadeId = c.ComunidadeId,
                NomeComunidade = c.Comunidade.Name,
                Texto = c.Texto,
                CriadoEm = c.CriadoEm,
                Visivel = c.Visivel,
                Reportado = c.Reportado,
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    [HttpPatch("{id:int}/reportar")]
    public async Task<IActionResult> Reportar(int id)
    {
        var comentario = await _db.Comentarios.FindAsync(id);

        if (comentario == null)
            return NotFound("Comentário não encontrado.");

        comentario.Reportado = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:int}/ocultar")]
    public async Task<IActionResult> Ocultar(int id)
    {
        var comentario = await _db.Comentarios.FindAsync(id);

        if (comentario == null)
            return NotFound("Comentário não encontrado.");

        comentario.Visivel = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var comentario = await _db.Comentarios.FindAsync(id);

        if (comentario == null)
            return NotFound("Comentário não encontrado.");

        _db.Comentarios.Remove(comentario);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
