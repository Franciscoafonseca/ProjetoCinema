using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PagamentoReferenciaMultibancoStrategy : IPagamentoStrategy
{
    public bool Suporta(string metodoPagamento) =>
        metodoPagamento.Equals("Multibanco", StringComparison.OrdinalIgnoreCase)
        || metodoPagamento.Equals("ReferenciaMultibanco", StringComparison.OrdinalIgnoreCase);

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
                Entidade = "12345",
                Valor = compra.ValorTotal,
                Metodo = "ReferenciaMultibanco",
                Estado = EstadoPagamento.Pendente,
                CriadoEm = dataPagamento,
                Mensagem =
                    $"Referencia Multibanco simulada: Entidade 12345, Referencia {referenciaMb[..3]} {referenciaMb.Substring(3, 3)} {referenciaMb[6..]}, Valor {compra.ValorTotal:0.00} EUR. Expira em 3 horas.",
            }
        );
    }
}
