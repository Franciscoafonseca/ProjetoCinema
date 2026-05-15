using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class FabricaAcessoUtilizador : IAcessoUtilizadorFactory
{
    private readonly IReadOnlyDictionary<
        TipoAcesso,
        IEstrategiaCriacaoAcessoUtilizador
    > _estrategias;

    public FabricaAcessoUtilizador(IEnumerable<IEstrategiaCriacaoAcessoUtilizador> estrategias)
    {
        _estrategias = estrategias.ToDictionary(e => e.Tipo);
    }

    public AcessoUtilizador Criar(
        int utilizadorId,
        Compra compra,
        CarrinhoItem item,
        DateTime dataCompra
    )
    {
        var tipo = item.Acesso.Tipo;

        if (!_estrategias.TryGetValue(tipo, out var estrategia))
            throw new InvalidOperationException(
                $"Não existe estratégia para o tipo de acesso '{tipo}'."
            );

        return estrategia.Criar(utilizadorId, compra, item, dataCompra);
    }
}
