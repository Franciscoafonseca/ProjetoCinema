using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IComentarioRepository
{
    Task<Comentario> AddAsync(Comentario comentario);

    Task<IEnumerable<Comentario>> ObterPorComunidadeIdAsync(int comunidadeId);

    Task<IEnumerable<Comentario>> ObterPorComunidadeIdAsync(
        int comunidadeId,
        bool incluirModerados
    );

    Task<IEnumerable<Comentario>> ObterPorFilmeIdAsync(int filmeId);

    Task<Comentario?> ObterPorIdAsync(int id);

    Task SaveChangesAsync();
}
