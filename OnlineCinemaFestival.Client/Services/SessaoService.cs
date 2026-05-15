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

    public async Task<List<SessaoDto>> GetSessoesAsync()
    {
        return await _http.GetFromJsonAsync<List<SessaoDto>>("api/sessoes") ?? new();
    }

    public async Task<List<SessaoDto>> GetSessoesDisponiveisAsync()
    {
        return await _http.GetFromJsonAsync<List<SessaoDto>>("api/sessoes/disponiveis") ?? new();
    }

    public async Task<SessaoDto?> GetSessaoAsync(int id)
    {
        var resposta = await _http.GetAsync($"api/sessoes/{id}");

        if (!resposta.IsSuccessStatusCode)
            return null;

        return await resposta.Content.ReadFromJsonAsync<SessaoDto>();
    }

    public async Task<SessaoEstadoDto?> GetEstadoAsync(int id)
    {
        var resposta = await _http.GetAsync($"api/sessoes/{id}/estado");

        if (!resposta.IsSuccessStatusCode)
            return null;

        return await resposta.Content.ReadFromJsonAsync<SessaoEstadoDto>();
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
