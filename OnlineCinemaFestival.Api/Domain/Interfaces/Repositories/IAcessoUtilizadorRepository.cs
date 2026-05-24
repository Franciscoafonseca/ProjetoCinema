using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IAcessoUtilizadorRepository
{
    Task AddRangeAsync(IEnumerable<AcessoUtilizador> acessos);

    Task<IEnumerable<AcessoUtilizador>> ObterPorUtilizadorIdAsync(int utilizadorId);

    Task<IEnumerable<AcessoUtilizador>> ObterAtivosPorUtilizadorIdAsync(
        int utilizadorId,
        DateTime dataAtual
    );

    Task<bool> ExisteAcessoAtivoAsync(int utilizadorId, int acessoId, DateTime dataAtual);

    Task<bool> ExisteParaCompraAsync(int compraId);
}
