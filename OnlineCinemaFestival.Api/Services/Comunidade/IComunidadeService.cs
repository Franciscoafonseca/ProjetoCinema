using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IComunidadeService
{
    Task<IEnumerable<ComunidadeReadDTO>> ObterTodasComunidadesAsync(int utilizadorIdPedido);

    Task<IEnumerable<ComunidadeReadDTO>> ObterMinhasComunidadesAsync(int utilizadorId);

    Task<ComunidadeReadDTO?> ObterComunidadePorPublicIdAsync(Guid publicId, int utilizadorIdPedido);

    Task<ComunidadeReadDTO> CriarComunidadeAsync(ComunidadeCreateDTO dto, int criadorUserId);

    Task<ComunidadeReadDTO?> ObterComunidadePorConviteAsync(
        string codigoConvite,
        int utilizadorIdPedido
    );

    Task AderirComunidadeAsync(Guid comunidadePublicId, int utilizadorId);

    Task AderirComunidadePorConviteAsync(string codigoConvite, int utilizadorId);

    Task ApagarComunidadeAsync(Guid comunidadePublicId, int utilizadorId);
    Task SairComunidadeAsync(Guid comunidadePublicId, int utilizadorId);
    Task<ComunidadeReadDTO> EnviarImagemAsync(Guid comunidadePublicId, int utilizadorId, IFormFile ficheiro);
}
