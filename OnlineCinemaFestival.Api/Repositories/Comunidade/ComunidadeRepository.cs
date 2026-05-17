using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class ComunidadeRepository : IComunidadeRepository
{
    private readonly AppDbContext _context;

    public ComunidadeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Comunidade> AddComunidadeAsync(Comunidade comunidade)
    {
        _context.Comunidades.Add(comunidade);
        await _context.SaveChangesAsync();
        return comunidade;
    }

    public async Task<Comunidade?> GetComunidadeByPublicIdAsync(Guid publicId)
    {
        return await _context
            .Comunidades.Include(c => c.CreatedByUser)
            .Include(c => c.Members)
            .Include(c => c.Comentarios)
            .FirstOrDefaultAsync(c => c.PublicId == publicId);
    }

    public async Task<IEnumerable<Comunidade>> FindComunidadesAsync(
        Expression<Func<Comunidade, bool>> predicate
    )
    {
        return await _context
            .Comunidades.Include(c => c.CreatedByUser)
            .Include(c => c.Members)
            .Include(c => c.Comentarios)
            .Where(predicate)
            .OrderByDescending(c => c.Members.Count)
            .ThenByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsMembroAsync(int comunidadeId, int utilizadorId)
    {
        return await _context.ComunidadeMembros.AnyAsync(cm =>
            cm.ComunidadeId == comunidadeId && cm.UtilizadorId == utilizadorId
        );
    }

    public async Task<Comunidade?> GetComunidadeByConviteAsync(string codigoConvite)
    {
        return await _context
            .Comunidades.Include(c => c.CreatedByUser)
            .Include(c => c.Members)
            .Include(c => c.Comentarios)
            .FirstOrDefaultAsync(c => c.CodigoConvite == codigoConvite);
    }

    public async Task<ComunidadeMembro> AdicionarMembroAsync(ComunidadeMembro membro)
    {
        var result = await _context.ComunidadeMembros.AddAsync(membro);
        await _context.SaveChangesAsync();
        return result.Entity;
    }
}
