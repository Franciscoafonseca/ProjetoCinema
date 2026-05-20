using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IAutenticacaoExternaService
{
    Task<List<ProvedorAutenticacaoExternaDTO>> ObterProvedoresAsync();

    Task<AutenticacaoRespostaDTO> AutenticarAsync(PedidoAutenticacaoExternaDTO request);
}
