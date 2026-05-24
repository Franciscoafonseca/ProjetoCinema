using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IComentarioRepository
{
    Task<Comentario> AddAsync(Comentario comentario);

    Task<Comentario?> GetByIdAsync(int comentarioId);

    Task<IEnumerable<Comentario>> ObterPorComunidadeIdAsync(
        int comunidadeId,
        bool incluirModerados = false
    );

    Task<IEnumerable<Comentario>> ObterReportadosPorComunidadeIdAsync(int comunidadeId);

    Task<IEnumerable<Comentario>> ObterPorFilmeIdAsync(int filmeId);

    Task UpdateAsync(Comentario comentario);
}
