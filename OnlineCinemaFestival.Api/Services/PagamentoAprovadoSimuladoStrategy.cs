using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PagamentoAprovadoSimuladoStrategy : IPagamentoStrategy
{
    private static readonly HashSet<string> Metodos = new(StringComparer.OrdinalIgnoreCase)
    {
        MetodosPagamento.CartaoCredito,
        MetodosPagamento.PayPal,
        MetodosPagamento.MBWay,
        MetodosPagamento.ApplePay,
        MetodosPagamento.GooglePay,
    };

    public bool Suporta(string metodoPagamento) =>
        string.IsNullOrWhiteSpace(metodoPagamento) || Metodos.Contains(metodoPagamento.Trim());

    public Task<Pagamento> ProcessarAsync(
        Compra compra,
        DateTime dataPagamento,
        string metodoPagamento
    )
    {
        var metodo = string.IsNullOrWhiteSpace(metodoPagamento)
            ? MetodosPagamento.CartaoCredito
            : metodoPagamento.Trim();

        return Task.FromResult(
            new Pagamento
            {
                Compra = compra,
                Referencia = $"PG-{compra.Referencia}",
                Valor = compra.ValorTotal,
                Metodo = metodo,
                Estado = EstadoPagamento.Aprovado,
                CriadoEm = dataPagamento,
                ProcessadoEm = dataPagamento,
                Mensagem = "Pagamento simulado aprovado automaticamente.",
            }
        );
    }
}
