using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class SessaoService
{
    private readonly HttpClient _http;

    public SessaoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<SessaoDto>> GetSessoesPorFilmeAsync(int filmeId)
    {
        var resposta = await _http.GetAsync($"api/sessoes/filme/{filmeId}");

        if (!resposta.IsSuccessStatusCode)
            return new();

        return await resposta.Content.ReadFromJsonAsync<List<SessaoDto>>() ?? new();
    }

    public async Task<List<SessaoDto>> GetSessoesPorFestivalAsync(int festivalId)
    {
        var resposta = await _http.GetAsync($"api/sessoes/festival/{festivalId}");

        if (!resposta.IsSuccessStatusCode)
            return new();

        return await resposta.Content.ReadFromJsonAsync<List<SessaoDto>>() ?? new();
    }
}
