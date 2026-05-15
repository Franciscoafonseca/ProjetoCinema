using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PagamentoSimuladoService : IPagamentoService
{
    public Task<Pagamento> ProcessarPagamentoSimuladoAsync(Compra compra, DateTime dataPagamento)
    {
        var pagamento = new Pagamento
        {
            Compra = compra,
            Referencia = $"PG-{compra.Referencia}",
            Valor = compra.ValorTotal,
            Metodo = "Simulado",
            Estado = EstadoPagamento.Aprovado,
            CriadoEm = dataPagamento,
            ProcessadoEm = dataPagamento,
            Mensagem = "Pagamento simulado aprovado automaticamente.",
        };

        return Task.FromResult(pagamento);
    }
}
