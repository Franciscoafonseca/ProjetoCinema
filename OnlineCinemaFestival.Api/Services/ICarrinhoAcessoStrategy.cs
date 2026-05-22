using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface ICarrinhoAcessoStrategy
{
    TipoAcesso Tipo { get; }

    bool PermiteQuantidadeMultipla { get; }

    void ValidarPedido(CarrinhoItemCreateDTO dto);

    Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto);

    void ValidarAcesso(Acesso acesso);
}
