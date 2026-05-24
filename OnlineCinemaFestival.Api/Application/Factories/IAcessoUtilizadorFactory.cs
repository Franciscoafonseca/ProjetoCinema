using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoUtilizadorFactory
{
    AcessoUtilizador Criar(int utilizadorId, Compra compra, CarrinhoItem item, DateTime dataCompra);
}
