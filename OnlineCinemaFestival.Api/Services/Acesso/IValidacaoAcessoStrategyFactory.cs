using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.Acesso;

public interface IValidacaoAcessoStrategyFactory
{
    IEstrategiaValidacaoAcesso ObterEstrategia(TipoAcesso tipo);

    IEnumerable<IEstrategiaValidacaoAcesso> ObterTodas();
}
