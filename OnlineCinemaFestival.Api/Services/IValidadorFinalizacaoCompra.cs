using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IValidadorFinalizacaoCompra
{
    Task ValidarAsync(int utilizadorId, Carrinho? carrinho);
}
