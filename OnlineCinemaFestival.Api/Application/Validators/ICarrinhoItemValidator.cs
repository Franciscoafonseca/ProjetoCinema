using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface ICarrinhoItemValidator
{
    TipoAcesso Tipo { get; }

    bool PermiteQuantidadeMultipla { get; }

    void ValidarPedido(CarrinhoItemCreateDTO dto);

    Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto);

    Task<Acesso?> ObterAcessoAtivoAsync(CarrinhoItemCreateDTO dto);

    void ValidarAcesso(Acesso acesso);
}
