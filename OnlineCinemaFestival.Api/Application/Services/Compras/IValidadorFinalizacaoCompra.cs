using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IValidadorFinalizacaoCompra
{
    Task ValidarAsync(int utilizadorId, Carrinho? carrinho);
}
