using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoUtilizadorFactory
{
    AcessoUtilizador Criar(int utilizadorId, Compra compra, ItemCarrinho item, DateTime dataCompra);
}
