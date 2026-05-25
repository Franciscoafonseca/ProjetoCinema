using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IPagamentoService
{
    Task<Pagamento> ProcessarPagamentoSimuladoAsync(Compra compra, string metodoPagamento);
}
