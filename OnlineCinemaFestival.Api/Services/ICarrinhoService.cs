using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICarrinhoService
{
    Task<CarrinhoReadDto> ObterCarrinhoAsync(int utilizadorId);

    Task<CarrinhoReadDto> AdicionarItemAsync(int utilizadorId, AdicionarItemCarrinhoDto dto);

    Task<CarrinhoReadDto> AdicionarItemAsync(int utilizadorId, CarrinhoItemCreateDto dto);

    Task<CarrinhoReadDto> AtualizarItemAsync(
        int utilizadorId,
        int itemId,
        CarrinhoItemUpdateDto dto
    );

    Task RemoverItemAsync(int utilizadorId, int itemId);

    Task LimparCarrinhoAsync(int utilizadorId);

    Task<CarrinhoValidacaoDto> ValidarCarrinhoAsync(int utilizadorId);

    Task<CarrinhoResumoDto> ObterResumoAsync(int utilizadorId);
}
