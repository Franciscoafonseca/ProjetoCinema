using Microsoft.Extensions.Configuration;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;
using OnlineCinemaFestival.Tests.Support.Assertions;
using OnlineCinemaFestival.Tests.Support.Builders;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Pagamentos;

public class MultibancoExpiracaoTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 24, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task ProcessarPagamento_Multibanco_GeraReferenciaPendente()
    {
        var pagamento = await CriarPagamentoService(new FakeTimeProvider(Agora))
            .ProcessarPagamentoSimuladoAsync(CriarCompra(), MetodosPagamento.ReferenciaMultibanco);

        pagamento.DeveEstarPendente();
        Assert.Equal("12345", pagamento.Entidade);
    }

    [Fact]
    public async Task ExpirarPendentes_TresHorasApos_MarcaComoExpirado()
    {
        var repo = new CompraRepositoryFalso();
        await repo.AddAsync(CriarCompraPendente(Agora.UtcDateTime));
        var service = CriarPagamentosPendentesService(repo, Agora.AddHours(3).AddSeconds(1));

        var expirados = await service.ExpirarPendentesAsync();
        var compra = (await repo.ObterPorIdAsync(1))!;

        Assert.Equal(1, expirados);
        compra.Pagamento!.DeveEstarExpirado();
        compra.DeveEstarCancelada();
    }

    [Fact]
    public async Task ConfirmarPagamento_AposExpiracao_Rejeita()
    {
        var repo = new CompraRepositoryFalso();
        await repo.AddAsync(CriarCompraPendente(Agora.UtcDateTime));
        var service = CriarPagamentosPendentesService(repo, Agora.AddHours(4));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ConfirmarPagamentoAsync(1, 7)
        );
    }

    [Fact]
    public async Task ObterDoUtilizador_PendentesAtivos_AparecemNaArea()
    {
        var repo = new CompraRepositoryFalso();
        await repo.AddAsync(CriarCompraPendente(Agora.UtcDateTime));
        var service = CriarPagamentosPendentesService(repo, Agora.AddHours(1));

        var pagamentos = await service.ObterDoUtilizadorAsync(7);

        var pagamento = Assert.Single(pagamentos);
        Assert.Equal(EstadoPagamento.Pendente, pagamento.Pagamento!.Estado);
    }

    private static Compra CriarCompra() => new CompraBuilder()
        .ComId(1)
        .ComReferencia("CMP-1")
        .DoUtilizador(7)
        .ComValorTotal(15m)
        .Build();

    private static Compra CriarCompraPendente(DateTime criadoEm)
    {
        var compra = new CompraBuilder()
            .ComId(1)
            .ComReferencia("CMP-1")
            .DoUtilizador(7)
            .ComValorTotal(15m)
            .NoEstado(EstadoCompra.Pendente)
            .Build();

        compra.Pagamento = new PagamentoBuilder()
            .ComoMultibancoPendente()
            .ComValor(compra.ValorTotal)
            .CriadoEm(criadoEm)
            .Build();
        compra.Pagamento.Compra = compra;

        return compra;
    }

    private static IPagamentoService CriarPagamentoService(TimeProvider timeProvider)
    {
        return new PagamentoSimuladoService(
            new IPagamentoStrategy[]
            {
                new PagamentoAprovadoSimuladoStrategy(),
                new PagamentoReferenciaMultibancoStrategy(CriarConfiguracaoPagamentos()),
            },
            timeProvider
        );
    }

    private static PagamentosPendentesService CriarPagamentosPendentesService(
        ICompraRepository repo,
        DateTimeOffset agora
    )
    {
        return new PagamentosPendentesService(repo, CriarConfiguracaoPagamentos(), new FakeTimeProvider(agora));
    }

    private static IConfiguration CriarConfiguracaoPagamentos()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Pagamentos:Multibanco:Entidade"] = "12345",
                    ["Pagamentos:Multibanco:ExpiracaoHoras"] = "3",
                }
            )
            .Build();
    }
}
