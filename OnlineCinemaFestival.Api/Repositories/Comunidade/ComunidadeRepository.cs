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
        return await ComunidadesComDetalhes().FirstOrDefaultAsync(c => c.PublicId == publicId);
    }

    public async Task<IEnumerable<Comunidade>> FindComunidadesAsync(
        Expression<Func<Comunidade, bool>> predicate
    )
    {
        return await ComunidadesComDetalhes()
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
        return await ComunidadesComDetalhes()
            .FirstOrDefaultAsync(c => c.CodigoConvite == codigoConvite);
    }

    public async Task<ComunidadeMembro> AdicionarMembroAsync(ComunidadeMembro membro)
    {
        var result = await _context.ComunidadeMembros.AddAsync(membro);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task ApagarComunidadeAsync(Comunidade comunidade)
    {
        _context.Comunidades.Remove(comunidade);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverMembroAsync(ComunidadeMembro membro)
    {
        _context.ComunidadeMembros.Remove(membro);
        await _context.SaveChangesAsync();
    }

    private IQueryable<Comunidade> ComunidadesComDetalhes()
    {
        return _context
            .Comunidades.AsSplitQuery()
            .Include(c => c.CreatedByUser)
            .Include(c => c.Members)
            .ThenInclude(m => m.Utilizador)
            .ThenInclude(u => u.Perfil)
            .Include(c => c.Comentarios);
    }
}
