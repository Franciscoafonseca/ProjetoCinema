using Microsoft.Extensions.Configuration;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;
using OnlineCinemaFestival.Tests.Support.Builders;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Acessos;

public class AcessoCompraServiceTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CriarAcessosSeAprovado_ChamadoDuasVezes_NaoDuplicaAcessos()
    {
        var acessoRepo = new AcessoUtilizadorRepositoryFalso();
        var timeProvider = new FakeTimeProvider(Agora);
        var service = new AcessoCompraService(
            new AcessoUtilizadorFactoryFalso(),
            acessoRepo,
            timeProvider
        );

        var acesso = new AcessoBuilder()
            .ComId(33)
            .ComNome("Bilhete Sessao")
            .ComPreco(10m)
            .Build();
        var compra = new CompraBuilder()
            .ComId(5)
            .DoUtilizador(7)
            .ComPagamento(new PagamentoBuilder().NoEstado(EstadoPagamento.Aprovado).Build())
            .Build();
        var carrinho = new Carrinho
        {
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

        var primeira = await service.CriarAcessosSeAprovadoAsync(7, compra, carrinho);
        var segunda = await service.CriarAcessosSeAprovadoAsync(7, compra, carrinho);

        Assert.Equal(2, primeira);
        Assert.Equal(0, segunda);
        Assert.Equal(2, acessoRepo.Adicionados.Count);
    }

    [Fact]
    public void FabricaAcessoUtilizador_AluguerDigital_CalculaJanelaTemporal()
    {
        var configuracao = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { ["Acessos:DuracaoAluguerDigitalHoras"] = "48" }
            )
            .Build();
        var fabrica = new FabricaAcessoUtilizador(
            new IEstrategiaCriacaoAcessoUtilizador[]
            {
                new EstrategiaCriacaoAluguerDigital(OpcoesTeste.Acessos()),
            }
        );
        var filme = new FilmeBuilder().ComId(3).ComTitulo("Filme").Build();
        var acesso = new AcessoBuilder()
            .ComId(44)
            .ComNome("Aluguer")
            .ComoAluguerDigital(filme, 24)
            .Build();
        var compra = new CompraBuilder().ComId(5).DoUtilizador(7).Build();
        var item = new CarrinhoItem { AcessoId = acesso.Id, Acesso = acesso };
        var dataCompra = Agora.UtcDateTime;

        var acessoUtilizador = fabrica.Criar(7, compra, item, dataCompra);

        Assert.Equal(dataCompra, acessoUtilizador.InicioValidade);
        Assert.Equal(dataCompra.AddHours(24), acessoUtilizador.FimValidade);
        Assert.Equal(3, acessoUtilizador.FilmeId);
    }
}
