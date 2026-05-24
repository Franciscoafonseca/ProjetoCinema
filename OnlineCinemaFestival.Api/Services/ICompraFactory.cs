using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraFactory
{
    Compra Criar(int utilizadorId, Carrinho carrinho);
}

