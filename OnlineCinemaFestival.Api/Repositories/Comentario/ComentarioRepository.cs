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

    public async Task<Comentario> AddAsync(Comentario comentario)
    {
        _context.Comentarios.Add(comentario);
        await _context.SaveChangesAsync();
        return comentario;
    }

    public async Task<IEnumerable<Comentario>> ObterPorComunidadeIdAsync(int comunidadeId)
    {
        return await ObterPorComunidadeIdAsync(comunidadeId, incluirModerados: false);
    }

    public async Task<IEnumerable<Comentario>> ObterPorComunidadeIdAsync(
        int comunidadeId,
        bool incluirModerados
    )
    {
        return await ComentariosComDetalhes()
            .Where(c => c.ComunidadeId == comunidadeId && (incluirModerados || c.Visivel))
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comentario>> ObterPorFilmeIdAsync(int filmeId)
    {
        return await ComentariosComDetalhes()
            .Where(c => c.FilmeId == filmeId && c.Visivel)
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync();
    }

    public async Task<Comentario?> ObterPorIdAsync(int id)
    {
        return await ComentariosComDetalhes().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<Comentario> ComentariosComDetalhes()
    {
        return _context
            .Comentarios.AsSplitQuery()
            .Include(c => c.Usuario)
                .ThenInclude(u => u.Perfil)
            .Include(c => c.Comunidade)
            .Include(c => c.Filme);
    }
}
