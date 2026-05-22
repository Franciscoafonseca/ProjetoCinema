using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PagamentoSimuladoService : IPagamentoService
{
    private readonly IEnumerable<IPagamentoStrategy> _strategies;

    public PagamentoSimuladoService(IEnumerable<IPagamentoStrategy> strategies)
    {
        _strategies = strategies;
    }

    public Task<Pagamento> ProcessarPagamentoSimuladoAsync(
        Compra compra,
        DateTime dataPagamento,
        string metodoPagamento
    )
    {
        var strategy = _strategies.FirstOrDefault(s => s.Suporta(metodoPagamento));

        if (strategy == null)
            throw new InvalidOperationException("Metodo de pagamento invalido.");

        return strategy.ProcessarAsync(compra, dataPagamento, metodoPagamento);
    }
}
