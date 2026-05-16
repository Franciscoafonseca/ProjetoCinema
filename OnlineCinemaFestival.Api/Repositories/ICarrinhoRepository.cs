using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface ICarrinhoRepository
{
    Task<Carrinho?> ObterPorUtilizadorIdAsync(int utilizadorId);

    Task<Carrinho> ObterOuCriarPorUtilizadorIdAsync(int utilizadorId);

    Task<CarrinhoItem?> ObterItemAsync(int carrinhoId, int itemId);

    Task<CarrinhoItem?> ObterItemPorAcessoAsync(int carrinhoId, int acessoId);

    Task<bool> ExisteItemComAcessoAsync(int carrinhoId, int acessoId);

    Task AddItemAsync(CarrinhoItem item);

    void RemoveItem(CarrinhoItem item);

    void RemoveItems(IEnumerable<CarrinhoItem> itens);

    Task SaveChangesAsync();
}
