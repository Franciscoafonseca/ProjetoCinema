using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IComentarioService
{
    Task<ComentarioReadDTO> CriarComentarioAsync(
        int comunidadeId,
        ComentarioCreateDTO dto,
        int usuarioId
    );
    Task<IEnumerable<ComentarioReadDTO>> ObterComentariosPorComunidadeIdAsync(int comunidadeId);
}
