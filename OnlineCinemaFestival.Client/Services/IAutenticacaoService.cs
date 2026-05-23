using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public interface IAutenticacaoService
{
    Task<AutenticacaoRespostaDTO> EntrarAsync(PedidoLoginDTO pedido);
    Task<AutenticacaoRespostaDTO> RegistarAsync(PedidoRegistoDTO pedido);
    Task<AutenticacaoRespostaDTO> EntrarComExternoAsync(PedidoAutenticacaoExternaDTO pedido);
    Task<List<ProvedorAutenticacaoExternaDTO>> ObterProvedoresExternosAsync();
    Task TerminarSessaoAsync();
}
