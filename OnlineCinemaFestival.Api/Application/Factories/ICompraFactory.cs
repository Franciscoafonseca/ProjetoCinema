using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraFactory
{
    Compra Criar(int utilizadorId, Carrinho carrinho);
}

