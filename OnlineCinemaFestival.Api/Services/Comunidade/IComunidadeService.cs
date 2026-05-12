using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IComunidadeService
{
    Task<IEnumerable<ComunidadeReadDto>> GetAllComunidadesAsync(int usuarioIdPedido);
    Task<IEnumerable<ComunidadeReadDto>> GetMinhasComunidadesAsync(int usuarioId);
    Task<ComunidadeReadDto?> GetComunidadeByPublicIdAsync(Guid publicId, int usuarioIdPedido);
    Task<ComunidadeReadDto> CreateComunidadeAsync(ComunidadeCreateDto dto, int criadorUserId);
    Task<ComunidadeReadDto?> GetComunidadeByConviteAsync(string codigoConvite);
}