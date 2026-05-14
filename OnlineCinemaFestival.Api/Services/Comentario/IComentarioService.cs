using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IComentarioService
{   
    // Uso Guid para nao expor o ID interno da comunidade guardado na BD
    Task<ComentarioReadDto> CriarComentarioAsync(Guid comunidadeId, ComentarioCreateDto dto, int usuarioId);
    Task<IEnumerable<ComentarioReadDto>> ObterComentariosPorComunidadeIdAsync(Guid comunidadeId, int usuarioId); 
}