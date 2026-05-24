using OnlineCinemaFestival.Api.Common.Errors;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Compras;

public class CheckoutValidacaoTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Validar_ComAcessoInativo_Rejeita()
    {
        var contexto = CarrinhoServiceTests.CriarContextoCarrinho();
        contexto.AcessoBilhete.IsAtivo = false;
        var carrinho = new Carrinho
        {
            UtilizadorId = 7,
            Itens = new List<CarrinhoItem>
            {
                new()
                {
                    AcessoId = contexto.AcessoBilhete.Id,
                    Acesso = contexto.AcessoBilhete,
                    PrecoUnitario = 10m,
                    Quantidade = 1,
                },
            },
        };
        var validador = new ValidadorFinalizacaoCompra(
            new AcessoUtilizadorRepositoryFalso(),
            contexto.Validators,
            new FakeTimeProvider(Agora),
            OpcoesTeste.Acessos()
        );

        await Assert.ThrowsAsync<ConflitoDominioException>(() =>
            validador.ValidarAsync(7, carrinho)
        );
    }

    [Fact]
    public async Task Validar_CarrinhoVazio_Rejeita()
    {
        var contexto = CarrinhoServiceTests.CriarContextoCarrinho();
        var validador = new ValidadorFinalizacaoCompra(
            new AcessoUtilizadorRepositoryFalso(),
            contexto.Validators,
            new FakeTimeProvider(Agora),
            OpcoesTeste.Acessos()
        );

        var ex = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            validador.ValidarAsync(7, new Carrinho { UtilizadorId = 7 })
        );

        Assert.Contains("vazio", ex.Message);
    }
}
