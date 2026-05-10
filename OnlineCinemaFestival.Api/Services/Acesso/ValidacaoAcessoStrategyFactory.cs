using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.Acesso;

public class ValidacaoAcessoStrategyFactory : IValidacaoAcessoStrategyFactory
{
    private readonly IEnumerable<IEstrategiaValidacaoAcesso> _estrategias;

    public ValidacaoAcessoStrategyFactory(IEnumerable<IEstrategiaValidacaoAcesso> estrategias)
    {
        _estrategias = estrategias;
    }

    public IEstrategiaValidacaoAcesso ObterEstrategia(TipoAcesso tipo)
    {
        var estrategia = _estrategias.FirstOrDefault(e => e.Tipo == tipo);

        if (estrategia == null)
            throw new ArgumentException("Tipo de acesso inválido.");

        return estrategia;
    }

    public IEnumerable<IEstrategiaValidacaoAcesso> ObterTodas()
    {
        return _estrategias.OrderBy(e => e.Tipo);
    }
}
