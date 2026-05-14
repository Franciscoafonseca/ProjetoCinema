using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services.Catalogo;

public class CatalogoOrdenacaoStrategyFactory
{
    private readonly IEnumerable<ICatalogoOrdenacaoStrategy> _strategies;

    public CatalogoOrdenacaoStrategyFactory(IEnumerable<ICatalogoOrdenacaoStrategy> strategies)
    {
        _strategies = strategies;
    }

    public ICatalogoOrdenacaoStrategy GetStrategy(CatalogoOrdenacao ordenacao)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Ordenacao == ordenacao);

        if (strategy == null)
            throw new ArgumentException("Critério de ordenação inválido.");

        return strategy;
    }
}
