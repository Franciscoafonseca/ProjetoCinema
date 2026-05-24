using Microsoft.Extensions.Options;
using OnlineCinemaFestival.Api.Configuracao;

namespace OnlineCinemaFestival.Tests.Support;

internal static class OpcoesTeste
{
    public static IOptions<AcessosOptions> Acessos(int quantidadeMaximaCarrinho = 5) =>
        Options.Create(
            new AcessosOptions
            {
                QuantidadeMaximaCarrinho = quantidadeMaximaCarrinho,
                DuracaoAluguerDigitalHoras = 48,
                DescontoFestival = 0.1m,
                ValidadePasseCompletoDias = 15,
                Precos = new Dictionary<string, decimal>
                {
                    [ChavesPrecosAcesso.AluguerDigital] = 3.99m,
                    [ChavesPrecosAcesso.PasseCompleto] = 24.99m,
                    [ChavesPrecosAcesso.PasseDiario] = 9.99m,
                    [ChavesPrecosAcesso.BilheteSessao] = 4.99m,
                    [ChavesPrecosAcesso.BilheteSessaoComChat] = 5.99m,
                },
            }
        );

    public static IOptions<PagamentoOptions> Pagamentos(
        string entidade = "12345",
        int expiracaoHoras = 3
    ) =>
        Options.Create(
            new PagamentoOptions
            {
                Multibanco = new MultibancoOptions
                {
                    Entidade = entidade,
                    ExpiracaoHoras = expiracaoHoras,
                },
            }
        );
}
