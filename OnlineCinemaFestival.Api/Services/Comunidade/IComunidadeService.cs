using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IComunidadeService
{
    Task<IEnumerable<ComunidadeReadDTO>> ObterTodasComunidadesAsync(int usuarioIdPedido);
    Task<IEnumerable<ComunidadeReadDTO>> ObterMinhasComunidadesAsync(int usuarioId);
    Task<ComunidadeReadDTO?> ObterComunidadePorIdAsync(int id, int usuarioIdPedido);
    Task<ComunidadeReadDTO> CreateComunidadeAsync(ComunidadeCreateDTO dto, int criadorUserId);
}
