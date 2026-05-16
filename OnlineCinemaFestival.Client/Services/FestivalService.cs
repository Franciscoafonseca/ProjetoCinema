using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class FestivalService
{
    private readonly HttpClient _http;

    public FestivalService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FestivalDTO>> ObterFestivaisAsync()
    {
        return await _http.GetFromJsonAsync<List<FestivalDTO>>("api/festivals") ?? new();
    }

    public async Task<FestivalDTO?> ObterFestivalPorIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<FestivalDTO>($"api/festivals/{id}");
    }

    public async Task<List<FilmeDTO>> ObterFilmesDoFestivalAsync(int festivalId)
    {
        return await _http.GetFromJsonAsync<List<FilmeDTO>>($"api/festivals/{festivalId}/filmes")
            ?? new();
    }
}
