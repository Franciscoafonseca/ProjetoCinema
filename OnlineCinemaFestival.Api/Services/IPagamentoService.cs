using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IPagamentoService
{
    Task<Pagamento> ProcessarPagamentoSimuladoAsync(Compra compra, DateTime dataPagamento);
}
