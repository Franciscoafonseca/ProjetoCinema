using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class CartService
{
    private readonly List<CompraItemDto> _items = new();
    private readonly CarrinhoApiService _api;

    public event Action? OnChange;

    public IReadOnlyList<CompraItemDto> Items => _items;

    public CartService(CarrinhoApiService api)
    {
        _api = api;
    }

    public async Task AddItemAsync(string utilizadorId, CompraItemDto item)
    {
        _items.Add(item);
        await _api.AddItemAsync(utilizadorId, item);
        NotifyChanged();
    }

    public async Task RemoveItemAsync(string utilizadorId, CompraItemDto item)
    {
        if (_items.Remove(item))
        {
            if (item.FilmeId.HasValue)
            {
                await _api.RemoveItemAsync(utilizadorId, item.FilmeId.Value);
            }

            NotifyChanged();
        }
    }

    public async Task ClearAsync(string utilizadorId)
    {
        if (_items.Count == 0)
        {
            return;
        }

        _items.Clear();
        await _api.ClearAsync(utilizadorId);
        NotifyChanged();
    }

    public async Task SyncAsync(string utilizadorId)
    {
        _items.Clear();
        _items.AddRange(await _api.GetAsync(utilizadorId));
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        OnChange?.Invoke();
    }
}
