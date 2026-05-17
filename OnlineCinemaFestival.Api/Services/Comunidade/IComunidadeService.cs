using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IComunidadeService
{
    Task<IEnumerable<ComunidadeReadDTO>> ObterTodasComunidadesAsync(int utilizadorIdPedido);

    Task<IEnumerable<ComunidadeReadDTO>> ObterMinhasComunidadesAsync(int utilizadorId);

    Task<ComunidadeReadDTO?> ObterComunidadePorPublicIdAsync(Guid publicId, int utilizadorIdPedido);

    Task<ComunidadeReadDTO> CriarComunidadeAsync(ComunidadeCreateDTO dto, int criadorUserId);

    Task<ComunidadeReadDTO?> ObterComunidadePorConviteAsync(string codigoConvite);

    Task AderirComunidadeAsync(Guid comunidadePublicId, int utilizadorId);

    Task AderirComunidadePorConviteAsync(string codigoConvite, int utilizadorId);
}
