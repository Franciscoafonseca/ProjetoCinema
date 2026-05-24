using OnlineCinemaFestival.Api.Application.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IAutenticacaoService
{
    Task<AutenticacaoRespostaDTO> RegistarAsync(PedidoRegistoDTO request);

    Task<AutenticacaoRespostaDTO> EntrarAsync(PedidoLoginDTO request);
}
