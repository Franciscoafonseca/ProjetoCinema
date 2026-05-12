using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IListaPessoalRepository
{
    Task<IEnumerable<ListaPessoal>> GetByUtilizadorAsync(int utilizadorId);

    Task<ListaPessoal?> GetByIdAsync(int id);

    Task AddAsync(ListaPessoal lista);

    Task<bool> FilmeExisteAsync(int filmeId);

    Task<ListaPessoalItem?> GetItemAsync(int listaId, int filmeId);

    Task AddItemAsync(ListaPessoalItem item);

    void RemoveItem(ListaPessoalItem item);

    Task SaveChangesAsync();
}
