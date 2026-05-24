using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public class PagamentoSimuladoService : IPagamentoService
{
    private readonly IEnumerable<IPagamentoStrategy> _strategies;
    private readonly TimeProvider _timeProvider;

    public PagamentoSimuladoService(IEnumerable<IPagamentoStrategy> strategies, TimeProvider timeProvider)
    {
        _strategies = strategies;
        _timeProvider = timeProvider;
    }

    public Task<Pagamento> ProcessarPagamentoSimuladoAsync(Compra compra, string metodoPagamento)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Suporta(metodoPagamento));
        var agora = _timeProvider.GetUtcNow().UtcDateTime;

        if (strategy == null)
        {
            return Task.FromResult(
                new Pagamento
                {
                    Compra = compra,
                    Referencia = $"PG-RECUSADO-{compra.Referencia}",
                    Valor = compra.ValorTotal,
                    Metodo = string.IsNullOrWhiteSpace(metodoPagamento)
                        ? "Invalido"
                        : metodoPagamento.Trim(),
                    Estado = EstadoPagamento.Recusado,
                    CriadoEm = agora,
                    ProcessadoEm = agora,
                    Mensagem = "Metodo de pagamento invalido ou recusado.",
                }
            );
        }

        return strategy.ProcessarAsync(compra, agora, metodoPagamento);
    }
}
