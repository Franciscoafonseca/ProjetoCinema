using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class ComentarioRepository : IComentarioRepository
{
    private readonly AppDbContext _context;

    public ComentarioRepository(AppDbContext context)
    {
        _context = context;
    }

    // 
    public async Task<Comentario> AddAsync(Comentario comentario)
    {
        _context.Comentarios.Add(comentario);
        await _context.SaveChangesAsync();
        return comentario;
    }

    public async Task<IEnumerable<Comentario>> GetByComunidadeIdAsync(int comunidadeId)
    {
        return await _context.Comentarios
            .Include(c => c.Usuario)
            .Where(c => c.ComunidadeId == comunidadeId && c.Visivel)
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync();
    }
}