using Microsoft.Extensions.Configuration;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Compras;

public class CompraHistoricoTests
{
    [Fact]
    public async Task Historico_DeveIncluirComprasPagasPendentesECanceladas()
    {
        var repo = new CompraRepositoryFalso();
        await repo.AddAsync(CriarCompra(1, EstadoCompra.Pago, EstadoPagamento.Aprovado, 30m));
        await repo.AddAsync(CriarCompra(2, EstadoCompra.Pendente, EstadoPagamento.Pendente, 15m));
        await repo.AddAsync(CriarCompra(3, EstadoCompra.Cancelado, EstadoPagamento.Recusado, 20m));
        var service = new CompraService(repo, OpcoesTeste.Pagamentos());

        var historico = (await service.ObterHistoricoDoUtilizadorAsync(7)).ToList();

        Assert.Equal(3, historico.Count);
        Assert.Contains(historico, c => c.EstadoNome == EstadoCompra.Pago.ToString());
        Assert.Contains(historico, c => c.EstadoNome == EstadoCompra.Pendente.ToString());
        Assert.Contains(historico, c => c.EstadoNome == EstadoCompra.Cancelado.ToString());
        Assert.Equal(3, historico.Single(c => c.EstadoNome == EstadoCompra.Pago.ToString()).PontosGanhos);
        Assert.Equal(0, historico.Single(c => c.EstadoNome == EstadoCompra.Cancelado.ToString()).PontosGanhos);
    }

    [Fact]
    public async Task MinhasCompras_DeveExporDadosDoPagamentoPersistido()
    {
        var repo = new CompraRepositoryFalso();
        await repo.AddAsync(CriarCompra(1, EstadoCompra.Pendente, EstadoPagamento.Pendente, 15m));
        var service = new CompraService(repo, OpcoesTeste.Pagamentos());

        var compra = (await service.ObterComprasDoUtilizadorAsync(7)).Single();

        Assert.NotNull(compra.Pagamento);
        Assert.Equal(EstadoPagamento.Pendente, compra.Pagamento!.Estado);
        Assert.Equal("REF-1", compra.Pagamento.Referencia);
    }

    private static Compra CriarCompra(
        int id,
        EstadoCompra estadoCompra,
        EstadoPagamento estadoPagamento,
        decimal total
    )
    {
        var acesso = new Acesso
        {
            Id = id,
            Nome = $"Acesso {id}",
            Tipo = TipoAcesso.AluguerDigital,
            FilmeId = id,
        };

        return new Compra
        {
            Id = id,
            Referencia = $"CMP-{id}",
            UtilizadorId = 7,
            ValorTotal = total,
            Estado = estadoCompra,
            CriadaEm = new DateTime(2026, 5, 23).AddHours(id),
            PagaEm = estadoCompra == EstadoCompra.Pago ? new DateTime(2026, 5, 23, 13, 0, 0) : null,
            Pagamento = new Pagamento
            {
                Referencia = $"REF-{id}",
                Valor = total,
                Estado = estadoPagamento,
                Metodo = "Teste",
                CriadoEm = new DateTime(2026, 5, 23),
            },
            Itens = new List<ItemCompra>
            {
                new()
                {
                    Id = id,
                    AcessoId = acesso.Id,
                    Acesso = acesso,
                    NomeAcesso = acesso.Nome,
                    TipoAcesso = acesso.Tipo,
                    PrecoUnitario = total,
                    Quantidade = 1,
                    Subtotal = total,
                },
            },
        };
    }

    private static IConfiguration CriarConfiguracao()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { ["Pagamentos:Multibanco:ExpiracaoHoras"] = "3" }
            )
            .Build();
    }
}
