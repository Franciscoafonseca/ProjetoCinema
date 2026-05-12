using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICarrinhoService
{
    Task<CarrinhoReadDto> ObterCarrinhoAsync(int utilizadorId);

    Task<CarrinhoReadDto> AdicionarItemAsync(int utilizadorId, AdicionarItemCarrinhoDto dto);

    Task RemoverItemAsync(int utilizadorId, int itemId);

    Task LimparCarrinhoAsync(int utilizadorId);
}
