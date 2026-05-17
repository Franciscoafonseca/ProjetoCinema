using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IComentarioService
{
    // Usa Guid para não expor o ID interno da comunidade guardado na BD.
    Task<ComentarioReadDTO> CriarComentarioAsync(
        Guid comunidadeId,
        ComentarioCreateDTO dto,
        int utilizadorId
    );
    Task<IEnumerable<ComentarioReadDTO>> ObterComentariosPorComunidadeIdAsync(
        Guid comunidadeId,
        int utilizadorId
    );
}
