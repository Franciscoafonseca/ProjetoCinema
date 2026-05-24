using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support.Builders;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Admin;

public class ReporteUtilizadorTests
{
    [Fact]
    public async Task Reportar_PrimeiroReporte_FicaPendente()
    {
        var (reportado, reporter, service, repo) = CriarContexto();

        var reporte = await service.ReportarAsync(
            reportado.Id,
            reporter.Id,
            new CriarReporteUtilizadorDTO { Motivo = "Comportamento abusivo no perfil publico." }
        );

        Assert.Equal(EstadoReporteUtilizador.Pendente, reporte.Estado);
        Assert.Equal(reportado.Id, reporte.UtilizadorReportadoId);
        Assert.Single(repo.Reportes);
    }

    [Fact]
    public async Task Reportar_MesmoPerfilDuasVezes_Rejeita()
    {
        var (reportado, reporter, service, _) = CriarContexto();

        await service.ReportarAsync(
            reportado.Id,
            reporter.Id,
            new CriarReporteUtilizadorDTO { Motivo = "Comportamento abusivo no perfil publico." }
        );

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ReportarAsync(
                reportado.Id,
                reporter.Id,
                new CriarReporteUtilizadorDTO { Motivo = "Novo reporte repetido do mesmo perfil." }
            )
        );
    }

    [Fact]
    public async Task ObterTodos_ListaReportadosComNomes()
    {
        var (reportado, reporter, service, _) = CriarContexto();
        await service.ReportarAsync(
            reportado.Id,
            reporter.Id,
            new CriarReporteUtilizadorDTO { Motivo = "Comportamento abusivo no perfil publico." }
        );

        var reportes = await service.ObterTodosAsync();

        var reporte = Assert.Single(reportes);
        Assert.Equal("Reportado", reporte.UtilizadorReportadoNome);
        Assert.Equal("Reporter", reporte.ReportadoPorUtilizadorNome);
    }

    [Fact]
    public async Task AtualizarEstado_AdminMudaParaRejeitado()
    {
        var (reportado, reporter, service, _) = CriarContexto();
        var criado = await service.ReportarAsync(
            reportado.Id,
            reporter.Id,
            new CriarReporteUtilizadorDTO { Motivo = "Comportamento abusivo no perfil publico." }
        );

        var atualizado = await service.AtualizarEstadoAsync(criado.Id, EstadoReporteUtilizador.Rejeitado);

        Assert.Equal(EstadoReporteUtilizador.Rejeitado, atualizado.Estado);
    }

    private static (
        Utilizador Reportado,
        Utilizador Reporter,
        ReporteUtilizadorService Service,
        ReporteUtilizadorRepositoryFalso Repo
    ) CriarContexto()
    {
        var reportado = new UtilizadorBuilder()
            .ComId(2)
            .ComNome("Reportado")
            .ComEmail("reportado@teste.pt")
            .Build();
        var reporter = new UtilizadorBuilder()
            .ComId(3)
            .ComNome("Reporter")
            .ComEmail("reporter@teste.pt")
            .Build();
        var repo = new ReporteUtilizadorRepositoryFalso();
        var service = new ReporteUtilizadorService(
            repo,
            new UtilizadorRepositoryFalso(reportado, reporter)
        );
        return (reportado, reporter, service, repo);
    }

    private sealed class ReporteUtilizadorRepositoryFalso : IReporteUtilizadorRepository
    {
        private int _sequencia;

        public List<ReporteUtilizador> Reportes { get; } = new();

        public Task<ReporteUtilizador?> ObterPorIdAsync(int id) =>
            Task.FromResult(Reportes.FirstOrDefault(r => r.Id == id));

        public Task<bool> ExisteAsync(int utilizadorReportadoId, int reportadoPorUtilizadorId) =>
            Task.FromResult(
                Reportes.Any(r =>
                    r.UtilizadorReportadoId == utilizadorReportadoId
                    && r.ReportadoPorUtilizadorId == reportadoPorUtilizadorId
                )
            );

        public Task<List<ReporteUtilizador>> ObterTodosAsync() =>
            Task.FromResult(Reportes.ToList());

        public Task AddAsync(ReporteUtilizador reporte)
        {
            reporte.Id = ++_sequencia;
            Reportes.Add(reporte);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
