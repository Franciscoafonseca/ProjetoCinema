using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;

namespace OnlineCinemaFestival.Tests.Rewards;

public class RewardsPontuacaoTests
{
    [Fact]
    public async Task Atribuir_PrimeiraVez_AdicionaSaldoEHistorico()
    {
        var rewards = new RewardsRepositoryFalso();
        var transacoes = new RewardTransacaoRepositoryFalso();
        var service = new RewardsPontuacaoService(
            rewards,
            transacoes,
            new FakeTimeProvider(new DateTimeOffset(2026, 5, 24, 10, 0, 0, TimeSpan.Zero))
        );

        var atribuido = await service.AtribuirSeAindaNaoAtribuidoAsync(
            5,
            3,
            "Comentario publicado",
            "comentario:10"
        );

        Assert.True(atribuido);
        Assert.Equal(3, rewards.ObterSaldo(5));
        Assert.Single(transacoes.ObterHistorico(5));
    }

    [Fact]
    public async Task Atribuir_MesmaAcaoSegundaVez_NaoDuplica()
    {
        var rewards = new RewardsRepositoryFalso();
        var transacoes = new RewardTransacaoRepositoryFalso();
        var service = new RewardsPontuacaoService(
            rewards,
            transacoes,
            new FakeTimeProvider(DateTimeOffset.UtcNow)
        );

        await service.AtribuirSeAindaNaoAtribuidoAsync(5, 3, "Comentario publicado", "comentario:10");
        var segunda = await service.AtribuirSeAindaNaoAtribuidoAsync(5, 3, "Comentario publicado", "comentario:10");

        Assert.False(segunda);
        Assert.Equal(3, rewards.ObterSaldo(5));
        Assert.Single(transacoes.ObterHistorico(5));
    }

    private sealed class RewardsRepositoryFalso : IRewardsRepository
    {
        private readonly Dictionary<int, int> _saldos = new();

        public Task AddOrUpdatePointsAsync(int utilizadorId, int pontosGanhos)
        {
            _saldos[utilizadorId] = ObterSaldo(utilizadorId) + pontosGanhos;
            return Task.CompletedTask;
        }

        public int ObterSaldo(int utilizadorId) =>
            _saldos.TryGetValue(utilizadorId, out var saldo) ? saldo : 0;
    }

    private sealed class RewardTransacaoRepositoryFalso : IRewardTransacaoRepository
    {
        private readonly List<RewardTransacao> _transacoes = new();

        public Task AddAsync(RewardTransacao transacao)
        {
            _transacoes.Add(transacao);
            return Task.CompletedTask;
        }

        public Task<bool> ExisteChaveAcaoAsync(int utilizadorId, string chaveAcao) =>
            Task.FromResult(_transacoes.Any(t => t.UtilizadorId == utilizadorId && t.ChaveAcao == chaveAcao));

        public List<RewardTransacao> ObterHistorico(int utilizadorId) =>
            _transacoes.Where(t => t.UtilizadorId == utilizadorId).ToList();

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
