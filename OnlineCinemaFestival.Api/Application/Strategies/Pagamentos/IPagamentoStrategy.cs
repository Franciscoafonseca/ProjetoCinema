using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IPagamentoStrategy
{
    bool Suporta(string metodoPagamento);

    Task<Pagamento> ProcessarAsync(Compra compra, DateTime dataPagamento, string metodoPagamento);
}
