using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public sealed class AcessoCompraService : IAcessoCompraService
{
    private readonly IAcessoUtilizadorFactory _fabricaAcessoUtilizador;
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;
    private readonly TimeProvider _timeProvider;

    public AcessoCompraService(
        IAcessoUtilizadorFactory fabricaAcessoUtilizador,
        IAcessoUtilizadorRepository acessoUtilizadorRepository,
        TimeProvider timeProvider
    )
    {
        _fabricaAcessoUtilizador = fabricaAcessoUtilizador;
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
        _timeProvider = timeProvider;
    }

    public async Task<int> CriarAcessosSeAprovadoAsync(int utilizadorId, Compra compra, Carrinho carrinho)
    {
        if (compra.Pagamento?.Estado != EstadoPagamento.Aprovado)
            return 0;

        if (compra.AcessosUtilizador.Any())
            return 0;

        if (compra.Id > 0 && await _acessoUtilizadorRepository.ExisteParaCompraAsync(compra.Id))
            return 0;

        var agora = _timeProvider.GetUtcNow().UtcDateTime;
        var acessos = carrinho.Itens
            .SelectMany(item =>
                Enumerable
                    .Range(0, item.Quantidade)
                    .Select(_ => _fabricaAcessoUtilizador.Criar(utilizadorId, compra, item, agora))
            )
            .ToList();

        if (acessos.Count > 0)
        {
            foreach (var acesso in acessos)
                compra.AcessosUtilizador.Add(acesso);

            await _acessoUtilizadorRepository.AddRangeAsync(acessos);
        }

        return acessos.Count;
    }
}

