using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PagamentoSimuladoService : IPagamentoService
{
    private static readonly HashSet<string> MetodosPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "CartaoCredito",
        "PayPal",
        "MBWay",
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

        var pagamento = new Pagamento
        {
            Compra = compra,
            Referencia = $"PG-{compra.Referencia}",
            Valor = compra.ValorTotal,
            Metodo = metodo,
            Estado = EstadoPagamento.Aprovado,
            CriadoEm = dataPagamento,
            ProcessadoEm = dataPagamento,
            Mensagem = "Pagamento simulado aprovado automaticamente.",
        };

        return Task.FromResult(pagamento);
    }
}
