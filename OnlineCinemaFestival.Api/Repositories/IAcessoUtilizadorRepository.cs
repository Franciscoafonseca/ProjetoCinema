using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IAcessoUtilizadorRepository
{
    Task AddRangeAsync(IEnumerable<AcessoUtilizador> acessos);

    Task<IEnumerable<AcessoUtilizador>> GetByUtilizadorIdAsync(int utilizadorId);

    Task<IEnumerable<AcessoUtilizador>> GetAtivosByUtilizadorIdAsync(
        int utilizadorId,
        DateTime dataAtual
    );

    Task<bool> ExisteAcessoAtivoAsync(int utilizadorId, int acessoId, DateTime dataAtual);
}
