using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IListaPessoalRepository
{
    Task<IEnumerable<ListaPessoal>> GetByUtilizadorAsync(int utilizadorId);

    Task<bool> ExisteNomeParaUtilizadorAsync(int utilizadorId, string nome);

    Task<ListaPessoal?> ObterPorIdAsync(int id);

    Task AddAsync(ListaPessoal lista);

    Task<bool> FilmeExisteAsync(int filmeId);

    Task<ListaPessoalItem?> ObterItemAsync(int listaId, int filmeId);

    Task AddItemAsync(ListaPessoalItem item);

    void RemoveItem(ListaPessoalItem item);

    void Remove(ListaPessoal lista);

    Task SaveChangesAsync();
}
