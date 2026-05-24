using Microsoft.Extensions.Configuration;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;

namespace OnlineCinemaFestival.Tests;

public class FinalizacaoCompraServiceTests
{
    [Fact]
    public async Task PagamentoAprovado_DeveCriarAcessos()
    {
        var contexto = CriarContexto(MetodosPagamento.CartaoCredito);

        var resultado = await contexto.Servico.FinalizarCompraAsync(7, MetodosPagamento.CartaoCredito);

        Assert.Equal(2, resultado.AcessosGerados);
        Assert.Equal(2, contexto.AcessoRepo.Adicionados.Count);
        Assert.Equal(EstadoCompra.Pago, contexto.CompraRepo.UltimaCompraAdicionada!.Estado);
        Assert.NotNull(contexto.CompraRepo.UltimaCompraAdicionada!.PagaEm);
        Assert.Equal(EstadoPagamento.Aprovado, contexto.CompraRepo.UltimaCompraAdicionada!.Pagamento!.Estado);
        Assert.True(contexto.CarrinhoCheckout.CarrinhoLimpado);
    }

    [Fact]
    public async Task PagamentoPendente_NaoDeveCriarAcessosAtivos()
    {
        var contexto = CriarContexto(MetodosPagamento.ReferenciaMultibanco);

        var resultado = await contexto.Servico.FinalizarCompraAsync(
            7,
            MetodosPagamento.ReferenciaMultibanco
        );

        Assert.Equal(0, resultado.AcessosGerados);
        Assert.Empty(contexto.AcessoRepo.Adicionados);
        Assert.Equal(EstadoCompra.Pendente, contexto.CompraRepo.UltimaCompraAdicionada!.Estado);
        Assert.Null(contexto.CompraRepo.UltimaCompraAdicionada!.PagaEm);
        Assert.Equal(EstadoPagamento.Pendente, contexto.CompraRepo.UltimaCompraAdicionada!.Pagamento!.Estado);
        Assert.True(contexto.CarrinhoCheckout.CarrinhoLimpado);
    }

    [Fact]
    public async Task PagamentoRecusado_NaoDeveCriarAcessos()
    {
        var contexto = CriarContexto("MetodoInexistente");

        var resultado = await contexto.Servico.FinalizarCompraAsync(7, "MetodoInexistente");

        Assert.Equal(0, resultado.AcessosGerados);
        Assert.Empty(contexto.AcessoRepo.Adicionados);
        Assert.Equal(EstadoCompra.Cancelado, contexto.CompraRepo.UltimaCompraAdicionada!.Estado);
        Assert.Null(contexto.CompraRepo.UltimaCompraAdicionada!.PagaEm);
        Assert.Equal(EstadoPagamento.Recusado, contexto.CompraRepo.UltimaCompraAdicionada!.Pagamento!.Estado);
        Assert.False(contexto.CarrinhoCheckout.CarrinhoLimpado);
    }

    [Fact]
    public async Task SegundaFinalizacao_AposCompraAprovada_NaoDuplicaAcessos()
    {
        var contexto = CriarContexto(MetodosPagamento.CartaoCredito);

        await contexto.Servico.FinalizarCompraAsync(7, MetodosPagamento.CartaoCredito);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            contexto.Servico.FinalizarCompraAsync(7, MetodosPagamento.CartaoCredito)
        );

        Assert.Equal(2, contexto.AcessoRepo.Adicionados.Count);
    }

    private static ContextoCheckout CriarContexto(string metodoPagamento)
    {
        var timeProvider = new FakeTimeProvider(
            new DateTimeOffset(2026, 5, 23, 12, 0, 0, TimeSpan.Zero)
        );
        var carrinho = CriarCarrinho();
        var compraRepo = new CompraRepositoryFalso();
        var acessoRepo = new AcessoUtilizadorRepositoryFalso();
        var carrinhoCheckout = new CarrinhoCheckoutServiceFalso(carrinho, timeProvider);
        var compraFactory = new CompraFactory(
            new GeradorReferenciaCompraFalso("CMP-TESTE-0001"),
            timeProvider
        );
        var acessoCompra = new AcessoCompraService(
            new AcessoUtilizadorFactoryFalso(),
            acessoRepo,
            timeProvider
        );
        var pagamentoService = CriarServicoPagamento(timeProvider);
        var configuracao = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Pagamentos:Multibanco:Entidade"] = "12345",
                    ["Pagamentos:Multibanco:ExpiracaoHoras"] = "3",
                }
            )
            .Build();

        var servico = new FinalizacaoCompraService(
            compraRepo,
            carrinhoCheckout,
            compraFactory,
            acessoCompra,
            pagamentoService,
            Array.Empty<ICompraObserver>(),
            configuracao
        );

        return new ContextoCheckout(servico, compraRepo, acessoRepo, carrinhoCheckout);
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
                new PagamentoReferenciaMultibancoStrategy(configuracao),
            },
            timeProvider
        );
    }

    private static Carrinho CriarCarrinho()
    {
        var acesso = new Acesso
        {
            Id = 33,
            Nome = "Bilhete Sessao",
            Tipo = TipoAcesso.BilheteSessao,
            IsAtivo = true,
            Preco = 10m,
        };

        var carrinho = new Carrinho
        {
            UtilizadorId = 7,
            Itens = new List<CarrinhoItem>
            {
                new()
                {
                    AcessoId = acesso.Id,
                    Acesso = acesso,
                    PrecoUnitario = 10m,
                    Quantidade = 2,
                },
            },
        };

        return carrinho;
    }

    private sealed record ContextoCheckout(
        FinalizacaoCompraService Servico,
        CompraRepositoryFalso CompraRepo,
        AcessoUtilizadorRepositoryFalso AcessoRepo,
        CarrinhoCheckoutServiceFalso CarrinhoCheckout
    );
}
