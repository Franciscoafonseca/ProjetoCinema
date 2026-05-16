using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICarrinhoService
{
    Task<CarrinhoReadDTO> ObterCarrinhoAsync(int utilizadorId);

    Task<CarrinhoReadDTO> AdicionarItemAsync(int utilizadorId, AdicionarItemCarrinhoDTO dto);

    Task<CarrinhoReadDTO> AdicionarItemAsync(int utilizadorId, CarrinhoItemCreateDTO dto);

    Task<CarrinhoReadDTO> AtualizarItemAsync(
        int utilizadorId,
        int itemId,
        CarrinhoItemUpdateDTO dto
    );

    Task RemoverItemAsync(int utilizadorId, int itemId);

    Task LimparCarrinhoAsync(int utilizadorId);

    Task<CarrinhoValidacaoDTO> ValidarCarrinhoAsync(int utilizadorId);

    Task<CarrinhoResumoDTO> ObterResumoAsync(int utilizadorId);
}
