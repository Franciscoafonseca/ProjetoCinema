using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IPerfilUtilizadorService
{
    Task<PerfilPrivadoDTO> ObterMeuPerfilAsync(int userId);

    Task<PerfilPrivadoDTO> AtualizarMeuPerfilAsync(int userId, PedidoAtualizarPerfilDTO request);

    Task<PerfilPrivadoDTO> EnviarFotoPerfilAsync(int userId, IFormFile ficheiro);

    Task<List<PerfilPublicoDTO>> ObterPerfisPublicosAsync();

    Task<PerfilPublicoDTO> ObterPerfilPublicoAsync(int userId);
}
