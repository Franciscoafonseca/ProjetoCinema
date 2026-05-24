using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;
using OnlineCinemaFestival.Tests.Support.Builders;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Premios;

public class VotacaoTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 24, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Votar_AntesDoInicioDaVotacao_Rejeita()
    {
        var repo = new PremioFestivalRepositoryFalso(CriarPremio(Agora.AddHours(1), Agora.AddHours(2)));
        var service = new PremioFestivalService(repo, new FakeTimeProvider(Agora), Array.Empty<IVotoPremioObserver>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.VotarAsync(1, 10, 7));
    }

    [Fact]
    public async Task Votar_DepoisDoFimDaVotacao_Rejeita()
    {
        var repo = new PremioFestivalRepositoryFalso(CriarPremio(Agora.AddHours(-2), Agora.AddHours(-1)));
        var service = new PremioFestivalService(repo, new FakeTimeProvider(Agora), Array.Empty<IVotoPremioObserver>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.VotarAsync(1, 10, 7));
    }

    [Fact]
    public async Task Votar_SemVisualizacoesNecessarias_RejeitaPorAutorizacao()
    {
        var repo = new PremioFestivalRepositoryFalso(CriarPremio(Agora.AddHours(-1), Agora.AddHours(1)))
        {
            ViuTodosFilmesElegiveis = false,
        };
        var service = new PremioFestivalService(repo, new FakeTimeProvider(Agora), Array.Empty<IVotoPremioObserver>());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.VotarAsync(1, 10, 7));
    }

    [Fact]
    public async Task Votar_ComVisualizacoesECompleto_RegistaVoto()
    {
        var repo = new PremioFestivalRepositoryFalso(CriarPremio(Agora.AddHours(-1), Agora.AddHours(1)))
        {
            ViuTodosFilmesElegiveis = true,
        };
        var service = new PremioFestivalService(repo, new FakeTimeProvider(Agora), Array.Empty<IVotoPremioObserver>());

        await service.VotarAsync(1, 10, 7);

        Assert.Single(repo.Votos);
    }

    [Fact]
    public async Task Votar_DuasVezesPeloMesmoUtilizador_Rejeita()
    {
        var repo = new PremioFestivalRepositoryFalso(CriarPremio(Agora.AddHours(-1), Agora.AddHours(1)))
        {
            ViuTodosFilmesElegiveis = true,
        };
        var service = new PremioFestivalService(repo, new FakeTimeProvider(Agora), Array.Empty<IVotoPremioObserver>());

        await service.VotarAsync(1, 10, 7);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.VotarAsync(1, 10, 7));
    }

    private static PremioFestival CriarPremio(DateTimeOffset inicio, DateTimeOffset fim)
    {
        var festival = new FestivalBuilder().ComId(99).ComNome("Festival").Build();
        return new PremioBuilder()
            .ComId(1)
            .NoFestival(festival)
            .ComNome("Premio do publico")
            .ComVotacao(inicio.UtcDateTime, fim.UtcDateTime)
            .NoEstado(EstadoPremio.Aberto)
            .Build();
    }
}
