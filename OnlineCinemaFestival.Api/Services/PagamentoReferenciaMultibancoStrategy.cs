using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PagamentoReferenciaMultibancoStrategy : IPagamentoStrategy
{
    private readonly string _entidade;
    private readonly int _expiracaoHoras;

    public PagamentoReferenciaMultibancoStrategy(IConfiguration configuration)
    {
        _entidade =
            configuration["Pagamentos:Multibanco:Entidade"]
            ?? throw new InvalidOperationException(
                "Pagamentos:Multibanco:Entidade nao configurada no appsettings.json."
            );

        _expiracaoHoras = PagamentosConfiguracao.ObterExpiracaoMultibancoHoras(configuration);
    }

    public bool Suporta(string metodoPagamento) =>
        metodoPagamento.Equals(MetodosPagamento.Multibanco, StringComparison.OrdinalIgnoreCase)
        || metodoPagamento.Equals(
            MetodosPagamento.ReferenciaMultibanco,
            StringComparison.OrdinalIgnoreCase
        );

    public Task<Pagamento> ProcessarAsync(
        Compra compra,
        DateTime dataPagamento,
        string metodoPagamento
    )
    {
        var referenciaBase =
            $"{Math.Abs(HashCode.Combine(compra.Referencia, compra.ValorTotal)):000000000}";
        var referenciaMb = referenciaBase.Length > 9
            ? referenciaBase[^9..]
            : referenciaBase.PadLeft(9, '0');

        return Task.FromResult(
            new Pagamento
            {
                Compra = compra,
                Referencia = referenciaMb,
                Entidade = _entidade,
                Valor = compra.ValorTotal,
                Metodo = MetodosPagamento.ReferenciaMultibanco,
                Estado = EstadoPagamento.Pendente,
                CriadoEm = dataPagamento,
                Mensagem =
                    $"Referencia Multibanco simulada: Entidade {_entidade}, Referencia {referenciaMb[..3]} {referenciaMb.Substring(3, 3)} {referenciaMb[6..]}, Valor {compra.ValorTotal:0.00} EUR. Expira em {_expiracaoHoras} horas.",
            }
        );
    }
}
