using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Infrastructure.Data;
using OnlineCinemaFestival.Api.Domain;

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

    public async Task<Comentario?> GetByIdAsync(int comentarioId)
    {
        return await ComentariosComDetalhes().FirstOrDefaultAsync(c => c.Id == comentarioId);
    }

    public async Task<IEnumerable<Comentario>> ObterPorComunidadeIdAsync(
        int comunidadeId,
        bool incluirModerados = false
    )
    {
        var query = ComentariosComDetalhes().Where(c => c.ComunidadeId == comunidadeId);

        if (!incluirModerados)
            query = query.Where(c => c.Visivel);

        return await query
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comentario>> ObterReportadosPorComunidadeIdAsync(int comunidadeId)
    {
        return await ComentariosComDetalhes()
            .Where(c => c.ComunidadeId == comunidadeId && c.Reportado)
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

    public async Task UpdateAsync(Comentario comentario)
    {
        _context.Comentarios.Update(comentario);
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
