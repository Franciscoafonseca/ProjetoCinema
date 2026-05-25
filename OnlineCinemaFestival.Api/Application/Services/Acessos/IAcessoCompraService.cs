using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoCompraService
{
    Task<int> CriarAcessosSeAprovadoAsync(int utilizadorId, Compra compra, Carrinho carrinho);
}

