using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface ICarrinhoRepository
{
    Task<Carrinho?> GetByUtilizadorIdAsync(int utilizadorId);

    Task<Carrinho> GetOrCreateByUtilizadorIdAsync(int utilizadorId);

    Task<CarrinhoItem?> GetItemAsync(int carrinhoId, int itemId);

    Task<CarrinhoItem?> GetItemByAcessoAsync(int carrinhoId, int acessoId);

    Task<bool> ExisteItemComAcessoAsync(int carrinhoId, int acessoId);

    Task AddItemAsync(CarrinhoItem item);

    void RemoveItem(CarrinhoItem item);

    void RemoveItems(IEnumerable<CarrinhoItem> itens);

    Task SaveChangesAsync();
}
