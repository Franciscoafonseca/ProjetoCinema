using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Infrastructure.Data;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Repositories;

public class ReporteUtilizadorRepository : IReporteUtilizadorRepository
{
    private readonly AppDbContext _context;

    public ReporteUtilizadorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ReporteUtilizador?> ObterPorIdAsync(int id)
    {
        return await ReportesComUtilizadores(asNoTracking: false)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> ExisteAsync(int utilizadorReportadoId, int reportadoPorUtilizadorId)
    {
        return await _context.ReportesUtilizadores.AnyAsync(r =>
            r.UtilizadorReportadoId == utilizadorReportadoId
            && r.ReportadoPorUtilizadorId == reportadoPorUtilizadorId
        );
    }

    public async Task<List<ReporteUtilizador>> ObterTodosAsync()
    {
        return await ReportesComUtilizadores()
            .OrderBy(r => r.Estado)
            .ThenByDescending(r => r.CriadoEm)
            .ToListAsync();
    }

    public async Task AddAsync(ReporteUtilizador reporte)
    {
        _context.ReportesUtilizadores.Add(reporte);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<ReporteUtilizador> ReportesComUtilizadores()
    {
        return ReportesComUtilizadores(asNoTracking: true);
    }

    private IQueryable<ReporteUtilizador> ReportesComUtilizadores(bool asNoTracking)
    {
        var query = _context.ReportesUtilizadores.AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        return query
            .Include(r => r.UtilizadorReportado)
            .Include(r => r.ReportadoPorUtilizador);
    }
}
