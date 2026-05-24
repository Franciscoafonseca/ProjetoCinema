using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IReporteUtilizadorRepository
{
    Task<ReporteUtilizador?> ObterPorIdAsync(int id);

    Task<bool> ExisteAsync(int utilizadorReportadoId, int reportadoPorUtilizadorId);

    Task<List<ReporteUtilizador>> ObterTodosAsync();

    Task AddAsync(ReporteUtilizador reporte);

    Task SaveChangesAsync();
}
