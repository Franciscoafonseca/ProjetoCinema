using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IValidadorCheckout
{
    Task ValidarAsync(int utilizadorId, Carrinho? carrinho);
}
