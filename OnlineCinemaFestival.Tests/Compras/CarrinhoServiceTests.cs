using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Common.Errors;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;
using OnlineCinemaFestival.Tests.Support.Builders;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Compras;

public class CarrinhoServiceTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task AdicionarItem_BilheteSessaoComQuantidade_CalculaTotal()
    {
        var contexto = CriarContextoCarrinho();

        var resultado = await contexto.Service.AdicionarItemAsync(
            7,
            new CarrinhoItemCreateDTO
            {
                TipoAcesso = TipoAcesso.BilheteSessao,
                SessaoId = 10,
                Quantidade = 3,
            }
        );

        Assert.Single(resultado.Itens);
        Assert.Equal(3, resultado.Itens[0].Quantidade);
        Assert.Equal(30m, resultado.Itens[0].Subtotal);
        Assert.Equal(30m, resultado.Total);
    }

    [Fact]
    public async Task ValidarPasseDiario_DataForaDoFestival_Rejeita()
    {
        var contexto = CriarContextoCarrinho();
        var validator = contexto.Validators.OfType<PasseDiarioCarrinhoItemValidator>().Single();

        var ex = await Assert.ThrowsAsync<ConflitoDominioException>(() =>
            validator.ValidarAlvoAsync(
                new CarrinhoItemCreateDTO
                {
                    TipoAcesso = TipoAcesso.PasseDiario,
                    FestivalId = 20,
                    DataAcesso = new DateTime(2026, 6, 1),
                    Quantidade = 1,
                }
            )
        );

        Assert.Contains("dentro do periodo do festival", ex.Message);
    }

    [Fact]
    public async Task ValidarAluguerDigital_FilmeInexistente_Rejeita()
    {
        var contexto = CriarContextoCarrinho();
        var validator = contexto.Validators.OfType<AluguerDigitalCarrinhoItemValidator>().Single();

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() =>
            validator.ValidarAlvoAsync(
                new CarrinhoItemCreateDTO
                {
                    TipoAcesso = TipoAcesso.AluguerDigital,
                    FilmeId = 999,
                    Quantidade = 1,
                }
            )
        );
    }

    internal static ContextoCarrinho CriarContextoCarrinho()
    {
        var timeProvider = new FakeTimeProvider(Agora);
        var festival = new FestivalBuilder()
            .ComId(20)
            .Entre(new DateTime(2026, 5, 23), new DateTime(2026, 5, 25))
            .Build();
        var filme = new FilmeBuilder().ComId(30).ComTitulo("Filme").Build();
        var sessao = new SessaoBuilder()
            .ComId(10)
            .NoFestival(festival)
            .DoFilme(filme)
            .Entre(new DateTime(2026, 5, 24, 20, 0, 0), new DateTime(2026, 5, 24, 22, 0, 0))
            .Build();

        var acessoBilhete = new AcessoBuilder()
            .ComId(100)
            .ComNome("Bilhete Sessao")
            .ComoBilheteSessao(sessao)
            .ComPreco(10m)
            .Build();
        var acessoPasseDiario = new AcessoBuilder()
            .ComId(101)
            .ComNome("Passe Diario")
            .ComoPasseDiario(festival, new DateTime(2026, 5, 24))
            .ComPreco(15m)
            .Build();
        var acessoPasseCompleto = new AcessoBuilder()
            .ComId(102)
            .ComNome("Passe Completo")
            .ComoPasseCompleto(festival)
            .ComPreco(40m)
            .Build();
        var acessoAluguer = new AcessoBuilder()
            .ComId(103)
            .ComNome("Aluguer")
            .ComoAluguerDigital(filme, 48)
            .ComPreco(5m)
            .Build();

        var acessoRepository = new AcessoRepositoryFalso(
            acessoBilhete,
            acessoPasseDiario,
            acessoPasseCompleto,
            acessoAluguer
        );

        var validators = new ICarrinhoItemValidator[]
        {
            new BilheteSessaoCarrinhoItemValidator(
                new SessaoRepositoryUnicoFalso(sessao),
                acessoRepository,
                timeProvider
            ),
            new PasseDiarioCarrinhoItemValidator(
                new FestivalRepositoryUnicoFalso(festival),
                acessoRepository,
                timeProvider
            ),
            new PasseCompletoCarrinhoItemValidator(
                new FestivalRepositoryUnicoFalso(festival),
                acessoRepository,
                timeProvider
            ),
            new AluguerDigitalCarrinhoItemValidator(
                new FilmeRepositoryUnicoFalso(filme),
                acessoRepository
            ),
        };

        var carrinhoRepository = new CarrinhoRepositoryFalso(7, acessoRepository.ObterPorIdLocal);
        var service = new CarrinhoService(
            carrinhoRepository,
            acessoRepository,
            new AcessoUtilizadorRepositoryFalso(),
            validators,
            timeProvider,
            OpcoesTeste.Acessos()
        );

        return new ContextoCarrinho(service, validators, acessoBilhete);
    }

    internal sealed record ContextoCarrinho(
        CarrinhoService Service,
        IReadOnlyCollection<ICarrinhoItemValidator> Validators,
        Acesso AcessoBilhete
    );
}
