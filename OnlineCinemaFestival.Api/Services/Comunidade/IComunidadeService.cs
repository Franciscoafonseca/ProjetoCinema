using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IComunidadeService
{
    Task<IEnumerable<ComunidadeReadDto>> GetAllComunidadesAsync(int usuarioIdPedido);
    Task<IEnumerable<ComunidadeReadDto>> GetMinhasComunidadesAsync(int usuarioId);
    Task<ComunidadeReadDto?> GetComunidadeByIdAsync(int id, int usuarioIdPedido);
    Task<ComunidadeReadDto> CreateComunidadeAsync(ComunidadeCreateDto dto, int criadorUserId);
}
