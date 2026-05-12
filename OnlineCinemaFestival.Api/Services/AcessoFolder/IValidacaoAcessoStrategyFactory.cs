using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.AcessosFolder;

public interface IValidacaoAcessoStrategyFactory
{
    IEstrategiaValidacaoAcesso ObterEstrategia(TipoAcesso tipo);

    IEnumerable<IEstrategiaValidacaoAcesso> ObterTodas();
}
