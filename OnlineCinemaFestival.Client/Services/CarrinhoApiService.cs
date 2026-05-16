using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class CarrinhoApiService
{
    private readonly HttpClient _http;

    public CarrinhoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CompraItemDto>> GetAsync(string utilizadorId)
    {
        var response = await _http.GetFromJsonAsync<List<CompraItemDto>>($"api/carrinho/{utilizadorId}");
        return response ?? new List<CompraItemDto>();
    }

    public async Task AddItemAsync(string utilizadorId, CompraItemDto item)
    {
        await _http.PostAsJsonAsync($"api/carrinho/{utilizadorId}/items", item);
    }

    public async Task RemoveItemAsync(string utilizadorId, int filmeId)
    {
        await _http.DeleteAsync($"api/carrinho/{utilizadorId}/items/{filmeId}");
    }

    public async Task ClearAsync(string utilizadorId)
    {
        await _http.DeleteAsync($"api/carrinho/{utilizadorId}");
    }
}
