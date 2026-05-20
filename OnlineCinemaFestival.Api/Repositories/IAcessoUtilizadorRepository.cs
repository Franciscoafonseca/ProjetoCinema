using OnlineCinemaFestival.Api.Models;

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

    Task<AcessoUtilizador?> ObterAcessoValidoAsync(
        int utilizadorId,
        TipoAcesso tipoAcesso,
        DateTime dataAtual,
        int? filmeId = null,
        int? sessaoId = null,
        int? festivalId = null
    );

    Task<AcessoUtilizador?> ObterPasseCompletoValidoParaFilmeAsync(
        int utilizadorId,
        int filmeId,
        int? festivalId,
        DateTime dataAtual
    );
}
