using OnlineCinemaFestival.Api.Models;

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

        if (strategy == null)
            throw new InvalidOperationException("Metodo de pagamento invalido.");

        var agora = _timeProvider.GetUtcNow().UtcDateTime;
        return strategy.ProcessarAsync(compra, agora, metodoPagamento);
    }
}
