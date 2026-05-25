using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services.ValidacaoAcesso;

public interface IValidacaoAcessoStrategyFactory
{
    IEstrategiaValidacaoAcesso ObterEstrategia(TipoAcesso tipo);

    IEnumerable<IEstrategiaValidacaoAcesso> ObterTodas();
}
