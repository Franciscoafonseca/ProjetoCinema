using Microsoft.Extensions.Configuration;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;

namespace OnlineCinemaFestival.Tests.Pagamentos;

public class PagamentoSimuladoTests
{
    [Fact]
    public async Task CartaoCredito_DeveFicarAprovado()
    {
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 5, 23, 12, 0, 0, TimeSpan.Zero));
        var service = CriarServicoPagamento(timeProvider);
        var compra = CriarCompra();

        var pagamento = await service.ProcessarPagamentoSimuladoAsync(
            compra,
            MetodosPagamento.CartaoCredito
        );

        Assert.Equal(EstadoPagamento.Aprovado, pagamento.Estado);
        Assert.Equal(MetodosPagamento.CartaoCredito, pagamento.Metodo);
        Assert.NotNull(pagamento.ProcessadoEm);
    }

    [Fact]
    public async Task ReferenciaMultibanco_DeveFicarPendenteEGerarReferencia()
    {
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 5, 23, 12, 0, 0, TimeSpan.Zero));
        var service = CriarServicoPagamento(timeProvider);
        var compra = CriarCompra();

        var pagamento = await service.ProcessarPagamentoSimuladoAsync(
            compra,
            MetodosPagamento.ReferenciaMultibanco
        );

        Assert.Equal(EstadoPagamento.Pendente, pagamento.Estado);
        Assert.Equal(MetodosPagamento.ReferenciaMultibanco, pagamento.Metodo);
        Assert.False(string.IsNullOrWhiteSpace(pagamento.Referencia));
        Assert.True(pagamento.Referencia.All(char.IsDigit));
        Assert.Equal("12345", pagamento.Entidade);
        Assert.Null(pagamento.ProcessadoEm);
    }

    [Fact]
    public async Task MetodoInvalido_DeveFicarRecusado()
    {
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 5, 23, 12, 0, 0, TimeSpan.Zero));
        var service = CriarServicoPagamento(timeProvider);
        var compra = CriarCompra();

        var pagamento = await service.ProcessarPagamentoSimuladoAsync(compra, "MetodoInexistente");

        Assert.Equal(EstadoPagamento.Recusado, pagamento.Estado);
        Assert.Equal("MetodoInexistente", pagamento.Metodo);
        Assert.NotNull(pagamento.ProcessadoEm);
        Assert.Contains("recusado", pagamento.Mensagem);
    }

    private static IPagamentoService CriarServicoPagamento(TimeProvider timeProvider)
    {
        var configuracao = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Pagamentos:Multibanco:Entidade"] = "12345",
                    ["Pagamentos:Multibanco:ExpiracaoHoras"] = "3",
                }
            )
            .Build();

        return new PagamentoSimuladoService(
            new IPagamentoStrategy[]
            {
                new PagamentoAprovadoSimuladoStrategy(),
                new PagamentoReferenciaMultibancoStrategy(OpcoesTeste.Pagamentos()),
            },
            timeProvider
        );
    }

    private static Compra CriarCompra()
    {
        return new Compra
        {
            Referencia = "CMP-TESTE-0001",
            ValorTotal = 12.5m,
        };
    }
}

