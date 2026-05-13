using System.Linq.Expressions;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IComunidadeRepository
{
    Task<Comunidade> AddComunidadeAsync(Comunidade comunidade);
    Task<Comunidade?> GetComunidadeByPublicIdAsync(Guid publicId);
    Task<IEnumerable<Comunidade>> FindComunidadesAsync(Expression<Func<Comunidade, bool>> predicate);
    Task<bool> IsMembroAsync(int comunidadeId, int usuarioId);
    Task<Comunidade?> GetComunidadeByConviteAsync(string codigoConvite);
    Task<ComunidadeMembro> AdicionarMembroAsync(ComunidadeMembro membro);
}