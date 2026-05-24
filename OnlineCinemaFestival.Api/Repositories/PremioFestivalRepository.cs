using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class PremioFestivalRepository : IPremioFestivalRepository
{
    private readonly AppDbContext _context;

    public PremioFestivalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> FestivalExisteAsync(int festivalId)
    {
        return await _context.Festivals.AnyAsync(f => f.Id == festivalId);
    }

    public async Task<PremioFestival?> ObterPremioAsync(int premioFestivalId)
    {
        return await _context.PremiosFestival.FirstOrDefaultAsync(p => p.Id == premioFestivalId);
    }

    public async Task<PremioFestival?> ObterPremioComResultadoAsync(int premioFestivalId)
    {
        return await _context
            .PremiosFestival.Include(p => p.Resultado)
            .FirstOrDefaultAsync(p => p.Id == premioFestivalId);
    }

    public async Task AddPremioAsync(PremioFestival premio)
    {
        await _context.PremiosFestival.AddAsync(premio);
    }

    public async Task<bool> FilmeElegivelAsync(int festivalId, int filmeId)
    {
        return await _context.FestivalFilmes.AnyAsync(ff =>
            ff.FestivalId == festivalId && ff.FilmeId == filmeId && ff.ElegivelPremiosPublico
        );
    }

    public async Task<bool> UtilizadorJaVotouAsync(int premioFestivalId, int utilizadorId)
    {
        return await _context.VotosPremiosFestival.AnyAsync(v =>
            v.PremioFestivalId == premioFestivalId && v.UtilizadorId == utilizadorId
        );
    }

    public async Task<bool> UtilizadorViuTodosFilmesElegiveisAsync(
        int festivalId,
        int utilizadorId
    )
    {
        var filmesElegiveis = await _context
            .FestivalFilmes.Where(ff =>
                ff.FestivalId == festivalId && ff.ElegivelPremiosPublico
            )
            .Select(ff => ff.FilmeId)
            .Distinct()
            .ToListAsync();

        if (filmesElegiveis.Count == 0)
            return false;

        var filmesVistosEmSessao = await _context
            .Visualizacoes.Where(v =>
                v.UtilizadorId == utilizadorId
                && v.FestivalId == festivalId
                && v.SessaoId != null
                && filmesElegiveis.Contains(v.FilmeId)
            )
            .Select(v => v.FilmeId)
            .Distinct()
            .CountAsync();

        return filmesVistosEmSessao == filmesElegiveis.Count;
    }

    public async Task AddVotoAsync(VotoPremioFestival voto)
    {
        await _context.VotosPremiosFestival.AddAsync(voto);
    }

    public async Task<(int FilmeId, int TotalVotos)?> ObterVencedorPorVotosAsync(
        int premioFestivalId
    )
    {
        var vencedor = await _context
            .VotosPremiosFestival.Where(v => v.PremioFestivalId == premioFestivalId)
            .GroupBy(v => v.FilmeId)
            .Select(g => new { FilmeId = g.Key, Total = g.Count() })
            .OrderByDescending(g => g.Total)
            .ThenBy(g => g.FilmeId)
            .FirstOrDefaultAsync();

        return vencedor == null ? null : (vencedor.FilmeId, vencedor.Total);
    }

    public async Task AddResultadoAsync(ResultadoPremioFestival resultado)
    {
        await _context.ResultadosPremiosFestival.AddAsync(resultado);
    }

    public async Task<ResultadoPremioFestival?> ObterResultadoCompletoAsync(int premioFestivalId)
    {
        return await _context
            .ResultadosPremiosFestival.Include(r => r.PremioFestival)
                .ThenInclude(p => p.Festival)
            .Include(r => r.FilmeVencedor)
            .FirstOrDefaultAsync(r => r.PremioFestivalId == premioFestivalId);
    }

    public async Task<List<ResultadoPremioFestival>> ObterResultadosPublicosAsync(
        int? festivalId,
        int? filmeId
    )
    {
        var query = _context
            .ResultadosPremiosFestival.Include(r => r.PremioFestival)
                .ThenInclude(p => p.Festival)
            .Include(r => r.FilmeVencedor)
            .Where(r => r.PremioFestival.EstadoPremio == EstadoPremio.Publicado);

        if (festivalId.HasValue)
            query = query.Where(r => r.PremioFestival.FestivalId == festivalId.Value);

        if (filmeId.HasValue)
            query = query.Where(r => r.FilmeIdVencedor == filmeId.Value);

        return await query
            .OrderByDescending(r => r.PublicadoEm)
            .ThenBy(r => r.PremioFestival.Nome)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PremioFestival>> ObterPremiosPorFestivalAsync(
        int festivalId,
        bool incluirRascunhos
    )
    {
        var query = _context.PremiosFestival.Where(p => p.FestivalId == festivalId);

        if (!incluirRascunhos)
            query = query.Where(p => p.EstadoPremio != EstadoPremio.Rascunho);

        return await query
            .OrderBy(p => p.DataAberturaVotacao)
            .ThenBy(p => p.Nome)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PremioFestival>> ObterPremiosPendentesPublicacaoAsync(DateTime dataAtual)
    {
        return await _context
            .PremiosFestival.Include(p => p.Festival)
            .Include(p => p.Resultado)
            .Where(p =>
                p.Resultado == null
                && p.DataFechoVotacao <= dataAtual
                && (p.EstadoPremio == EstadoPremio.Aberto || p.EstadoPremio == EstadoPremio.Fechado)
            )
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> TrySaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }
}
