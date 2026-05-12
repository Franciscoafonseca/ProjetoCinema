using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface ICarrinhoRepository
{
    Task<Carrinho?> GetByUtilizadorIdAsync(int utilizadorId);

    Task<Carrinho> GetOrCreateByUtilizadorIdAsync(int utilizadorId);

    Task<ItemCarrinho?> GetItemAsync(int carrinhoId, int itemId);

    Task<bool> ExisteItemComAcessoAsync(int carrinhoId, int acessoId);

    Task AddItemAsync(ItemCarrinho item);

    void RemoveItem(ItemCarrinho item);

    void RemoveItems(IEnumerable<ItemCarrinho> itens);

    Task SaveChangesAsync();
}
