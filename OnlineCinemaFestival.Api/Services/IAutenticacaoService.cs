using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IAutenticacaoService
{
    Task<AutenticacaoRespostaDTO> RegistarAsync(PedidoRegistoDTO request);

    Task<AutenticacaoRespostaDTO> EntrarAsync(PedidoLoginDTO request);
}
