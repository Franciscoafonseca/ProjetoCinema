using System.Linq.Expressions;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IComunidadeRepository
{
    Task<Comunidade> AddComunidadeAsync(Comunidade comunidade);
    Task<Comunidade?> ObterComunidadePorIdAsync(int id);
    Task<IEnumerable<Comunidade>> FindComunidadesAsync(
        Expression<Func<Comunidade, bool>> predicate
    );
    Task<bool> IsMembroAsync(int comunidadeId, int usuarioId);
}
