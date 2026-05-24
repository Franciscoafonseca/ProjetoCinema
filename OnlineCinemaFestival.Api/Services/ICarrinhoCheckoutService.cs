using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface ICarrinhoCheckoutService
{
    Task<Carrinho> ObterCarrinhoValidadoAsync(int utilizadorId);

    Task LimparCarrinhoAsync(Carrinho carrinho);
}

