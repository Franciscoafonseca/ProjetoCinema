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

        var agora = _timeProvider.GetUtcNow().UtcDateTime;
        var acessos = carrinho.Itens
            .Select(item => _fabricaAcessoUtilizador.Criar(utilizadorId, compra, item, agora))
            .ToList();

        if (acessos.Count > 0)
            await _acessoUtilizadorRepository.AddRangeAsync(acessos);

        return acessos.Count;
    }
}

