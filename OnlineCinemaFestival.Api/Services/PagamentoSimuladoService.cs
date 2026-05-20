using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PagamentoSimuladoService : IPagamentoService
{
    private static readonly HashSet<string> MetodosPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "CartaoCredito",
        "PayPal",
        "MBWay",
        "Multibanco",
        "ReferenciaMultibanco",
        "ApplePay",
        "GooglePay",
    };

    public Task<Pagamento> ProcessarPagamentoSimuladoAsync(
        Compra compra,
        DateTime dataPagamento,
        string metodoPagamento
    )
    {
        var metodo = string.IsNullOrWhiteSpace(metodoPagamento)
            ? "CartaoCredito"
            : metodoPagamento.Trim();

        if (!MetodosPermitidos.Contains(metodo))
            throw new InvalidOperationException("Metodo de pagamento invalido.");

        var isMultibanco =
            metodo.Equals("Multibanco", StringComparison.OrdinalIgnoreCase)
            || metodo.Equals("ReferenciaMultibanco", StringComparison.OrdinalIgnoreCase);

        var referenciaBase = $"{Math.Abs(HashCode.Combine(compra.Referencia, compra.ValorTotal)):000000000}";
        var referenciaMb = referenciaBase.Length > 9
            ? referenciaBase[^9..]
            : referenciaBase.PadLeft(9, '0');

        var pagamento = new Pagamento
        {
            Compra = compra,
            Referencia = isMultibanco ? referenciaMb : $"PG-{compra.Referencia}",
            Entidade = isMultibanco ? "12345" : null,
            Valor = compra.ValorTotal,
            Metodo = isMultibanco ? "ReferenciaMultibanco" : metodo,
            Estado = EstadoPagamento.Aprovado,
            CriadoEm = dataPagamento,
            ProcessadoEm = dataPagamento,
            Mensagem = isMultibanco
                ? $"Referencia Multibanco simulada: Entidade 12345, Referencia {referenciaMb[..3]} {referenciaMb.Substring(3, 3)} {referenciaMb[6..]}, Valor {compra.ValorTotal:0.00} EUR."
                : "Pagamento simulado aprovado automaticamente.",
        };

        return Task.FromResult(pagamento);
    }
}
