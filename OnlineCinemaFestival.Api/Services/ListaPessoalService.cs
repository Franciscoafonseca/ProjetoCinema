using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

/// <summary>
/// Serviço responsável pela gestão das listas pessoais dos utilizadores
/// (Quero ver, Vistos, Favoritos e listas personalizadas).
/// </summary>
public class ListaPessoalService : IListaPessoalService
{
    private readonly IListaPessoalRepository _repository;

    public ListaPessoalService(IListaPessoalRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Devolve todas as listas pessoais do utilizador autenticado.
    /// </summary>
    public async Task<IEnumerable<ListaPessoalReadDto>> GetMinhasListasAsync(int utilizadorId)
    {
        var listas = await _repository.GetByUtilizadorAsync(utilizadorId);
        return listas.Select(ListaPessoalMapper.MapToReadDto);
    }

    /// <summary>
    /// Cria uma nova lista pessoal para o utilizador autenticado.
    /// </summary>
    /// <exception cref="ArgumentException">Quando o nome é inválido.</exception>
    public async Task<ListaPessoalReadDto> CreateAsync(int utilizadorId, ListaPessoalCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("O nome da lista é obrigatório.");

        var lista = ListaPessoalMapper.MapFromCreateDto(dto, utilizadorId);

        await _repository.AddAsync(lista);
        await _repository.SaveChangesAsync();

        return ListaPessoalMapper.MapToReadDto(lista);
    }

    /// <summary>
    /// Adiciona um filme a uma lista do utilizador autenticado.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Lista ou filme inexistentes.</exception>
    /// <exception cref="UnauthorizedAccessException">Lista pertence a outro utilizador.</exception>
    /// <exception cref="InvalidOperationException">Filme já presente na lista.</exception>
    public async Task<ListaPessoalItemReadDto> AdicionarFilmeAsync(int utilizadorId, int listaId, int filmeId)
    {
        var lista = await GarantirAcessoLista(utilizadorId, listaId);

        if (!await _repository.FilmeExisteAsync(filmeId))
            throw new KeyNotFoundException("Filme não encontrado.");

        var jaExiste = await _repository.GetItemAsync(listaId, filmeId);

        if (jaExiste != null)
            throw new InvalidOperationException("O filme já se encontra nesta lista.");

        var item = new ListaPessoalItem
        {
            ListaPessoalId = listaId,
            FilmeId = filmeId,
            AddedAt = DateTime.UtcNow,
        };

        await _repository.AddItemAsync(item);
        lista.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync();

        var itemCompleto = await _repository.GetItemAsync(listaId, filmeId);
        return ListaPessoalMapper.MapToItemReadDto(itemCompleto!);
    }

    /// <summary>
    /// Remove um filme de uma lista do utilizador autenticado.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Lista ou item inexistentes.</exception>
    /// <exception cref="UnauthorizedAccessException">Lista pertence a outro utilizador.</exception>
    public async Task RemoverFilmeAsync(int utilizadorId, int listaId, int filmeId)
    {
        var lista = await GarantirAcessoLista(utilizadorId, listaId);

        var item = await _repository.GetItemAsync(listaId, filmeId);

        if (item == null)
            throw new KeyNotFoundException("Filme não encontrado nesta lista.");

        _repository.RemoveItem(item);
        lista.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync();
    }

    /// <summary>
    /// Verifica se a lista existe e pertence ao utilizador autenticado.
    /// </summary>
    private async Task<ListaPessoal> GarantirAcessoLista(int utilizadorId, int listaId)
    {
        var lista = await _repository.GetByIdAsync(listaId);

        if (lista == null)
            throw new KeyNotFoundException("Lista não encontrada.");

        if (lista.UtilizadorId != utilizadorId)
            throw new UnauthorizedAccessException("Não tens acesso a esta lista.");

        return lista;
    }
}
