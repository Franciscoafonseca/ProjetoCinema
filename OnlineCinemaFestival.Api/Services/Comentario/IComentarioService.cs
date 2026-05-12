using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IComentarioService
{
    Task<ComentarioReadDto> CriarComentarioAsync(
        int comunidadeId,
        ComentarioCreateDto dto,
        int usuarioId
    );
    Task<IEnumerable<ComentarioReadDto>> ObterComentariosPorComunidadeIdAsync(int comunidadeId);
}
