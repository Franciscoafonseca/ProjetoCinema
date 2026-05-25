using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public sealed class CarrinhoCheckoutService : ICarrinhoCheckoutService
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IValidadorFinalizacaoCompra _validadorFinalizacaoCompra;
    private readonly TimeProvider _timeProvider;

    public CarrinhoCheckoutService(
        ICarrinhoRepository carrinhoRepository,
        IValidadorFinalizacaoCompra validadorFinalizacaoCompra,
        TimeProvider timeProvider
    )
    {
        _carrinhoRepository = carrinhoRepository;
        _validadorFinalizacaoCompra = validadorFinalizacaoCompra;
        _timeProvider = timeProvider;
    }

    public async Task<Carrinho> ObterCarrinhoValidadoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        await _validadorFinalizacaoCompra.ValidarAsync(utilizadorId, carrinho);

        return carrinho!;
    }

    public async Task LimparCarrinhoAsync(Carrinho carrinho)
    {
        var agora = _timeProvider.GetUtcNow().UtcDateTime;

        _carrinhoRepository.RemoveItems(carrinho.Itens.ToList());
        carrinho.AtualizadoEm = agora;

        await _carrinhoRepository.SaveChangesAsync();
    }
}

