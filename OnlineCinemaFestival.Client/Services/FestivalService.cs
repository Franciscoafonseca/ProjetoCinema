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

    public async Task<List<FestivalDto>> GetFestivaisAsync()
    {
        return await _http.GetFromJsonAsync<List<FestivalDto>>("api/festivals") ?? new();
    }

    public async Task<FestivalDto?> GetFestivalPorIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<FestivalDto>($"api/festivals/{id}");
    }

    public async Task<List<FilmeDto>> GetFilmesDoFestivalAsync(int festivalId)
    {
        return await _http.GetFromJsonAsync<List<FilmeDto>>($"api/festivals/{festivalId}/filmes") ?? new();
    }
}
