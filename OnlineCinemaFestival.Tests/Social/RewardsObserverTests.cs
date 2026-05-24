using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;

namespace OnlineCinemaFestival.Tests.Social;

/// <summary>
/// Testa o padrão Observer de Rewards: cada observer notifica o serviço de pontuação
/// com a chave de ação correta, garantindo idempotência (sem duplicados).
/// </summary>
public class RewardsObserverTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 24, 10, 0, 0, TimeSpan.Zero);

    // ── ComentárioObserver ────────────────────────────────────────────────────

    [Fact]
    public async Task ComentarioObserver_AtribuiPontosPorComentario()
    {
        var (capturador, service) = CriarServicoPontuacao();
        var observer = new RewardsComentarioObserver(service);
        var comentario = new Comentario { Id = 42, UsuarioId = 5, Texto = "Bom filme." };

        await observer.NotificarAsync(comentario);

        Assert.Equal(1, capturador.ObterSaldo(5));
        Assert.Single(capturador.Historico(5));
        Assert.Contains("comentario:42", capturador.Historico(5)[0].ChaveAcao);
    }

    [Fact]
    public async Task ComentarioObserver_MesmoComentario_NaoDuplica()
    {
        var (capturador, service) = CriarServicoPontuacao();
        var observer = new RewardsComentarioObserver(service);
        var comentario = new Comentario { Id = 42, UsuarioId = 5, Texto = "Bom filme." };

        await observer.NotificarAsync(comentario);
        await observer.NotificarAsync(comentario);

        Assert.Equal(1, capturador.ObterSaldo(5));
        Assert.Single(capturador.Historico(5));
    }

    // ── AvaliaçãoObserver ─────────────────────────────────────────────────────

    [Fact]
    public async Task AvaliacaoObserver_AtribuiDoisPontosPorAvaliacao()
    {
        var (capturador, service) = CriarServicoPontuacao();
        var observer = new RewardsAvaliacaoObserver(service);
        var avaliacao = new Avaliacao { Id = 10, UsuarioId = 3, FilmeId = 7, Pontuacao = 4 };

        await observer.NotificarAsync(avaliacao);

        Assert.Equal(2, capturador.ObterSaldo(3));
        Assert.Single(capturador.Historico(3));
    }

    [Fact]
    public async Task AvaliacaoObserver_MesmoFilmeDuasVezes_NaoDuplica()
    {
        var (capturador, service) = CriarServicoPontuacao();
        var observer = new RewardsAvaliacaoObserver(service);
        var avaliacao = new Avaliacao { Id = 10, UsuarioId = 3, FilmeId = 7, Pontuacao = 4 };

        await observer.NotificarAsync(avaliacao);
        await observer.NotificarAsync(avaliacao);

        Assert.Equal(2, capturador.ObterSaldo(3));
        Assert.Single(capturador.Historico(3));
    }

    // ── VisualizaçãoObserver ──────────────────────────────────────────────────

    [Fact]
    public async Task VisualizacaoObserver_AtribuiPontoPorVisualizacaoFilme()
    {
        var (capturador, service) = CriarServicoPontuacao();
        var observer = new RewardsVisualizacaoObserver(service);
        var visualizacao = new Visualizacao { Id = 77, UtilizadorId = 9, FilmeId = 5 };

        await observer.NotificarAsync(visualizacao);

        Assert.Equal(1, capturador.ObterSaldo(9));
        Assert.Single(capturador.Historico(9));
        Assert.Contains("filme:5", capturador.Historico(9)[0].ChaveAcao);
    }

    [Fact]
    public async Task VisualizacaoObserver_AtribuiPontoPorVisualizacaoSessao()
    {
        var (capturador, service) = CriarServicoPontuacao();
        var observer = new RewardsVisualizacaoObserver(service);
        var visualizacao = new Visualizacao { Id = 78, UtilizadorId = 9, FilmeId = 5, SessaoId = 3 };

        await observer.NotificarAsync(visualizacao);

        Assert.Contains("sessao:3", capturador.Historico(9)[0].ChaveAcao);
    }

    // ── VotoPremioObserver ────────────────────────────────────────────────────

    [Fact]
    public async Task VotoPremioObserver_AtribuiDoisPontosPorVoto()
    {
        var (capturador, service) = CriarServicoPontuacao();
        var observer = new RewardsVotoPremioObserver(service);
        var voto = new VotoPremioFestival { Id = 1, UtilizadorId = 11, PremioFestivalId = 2 };

        await observer.NotificarAsync(voto);

        Assert.Equal(2, capturador.ObterSaldo(11));
        Assert.Single(capturador.Historico(11));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static (RewardsCapturador Capturador, RewardsPontuacaoService Servico) CriarServicoPontuacao()
    {
        var capturador = new RewardsCapturador();
        var servico = new RewardsPontuacaoService(
            capturador,
            capturador,
            new FakeTimeProvider(Agora)
        );
        return (capturador, servico);
    }

    // ── Fake duplo (IRewardsRepository + IRewardTransacaoRepository) ──────────

    private sealed class RewardsCapturador : IRewardsRepository, IRewardTransacaoRepository
    {
        private readonly Dictionary<int, int> _saldos = new();
        private readonly List<RewardTransacao> _transacoes = new();

        // IRewardsRepository
        public Task AddOrUpdatePointsAsync(int utilizadorId, int pontos)
        {
            _saldos[utilizadorId] = ObterSaldo(utilizadorId) + pontos;
            return Task.CompletedTask;
        }

        // IRewardTransacaoRepository
        public Task AddAsync(RewardTransacao t) { _transacoes.Add(t); return Task.CompletedTask; }
        public Task<bool> ExisteChaveAcaoAsync(int uid, string chave) =>
            Task.FromResult(_transacoes.Any(t => t.UtilizadorId == uid && t.ChaveAcao == chave));
        public List<RewardTransacao> ObterHistorico(int uid) =>
            _transacoes.Where(t => t.UtilizadorId == uid).ToList();
        public Task SaveChangesAsync() => Task.CompletedTask;

        // Helpers de assertion
        public int ObterSaldo(int uid) => _saldos.TryGetValue(uid, out var s) ? s : 0;
        public List<RewardTransacao> Historico(int uid) => ObterHistorico(uid);
    }
}
