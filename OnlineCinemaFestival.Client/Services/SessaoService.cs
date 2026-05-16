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

    public async Task<List<SessaoDTO>> ObterSessoesAsync()
    {
        return await _http.GetFromJsonAsync<List<SessaoDTO>>("api/sessoes") ?? new();
    }

    public async Task<List<SessaoDTO>> ObterSessoesDisponiveisAsync()
    {
        return await _http.GetFromJsonAsync<List<SessaoDTO>>("api/sessoes/disponiveis") ?? new();
    }

    public async Task<SessaoDTO?> ObterSessaoAsync(int id)
    {
        var resposta = await _http.GetAsync($"api/sessoes/{id}");

        if (!resposta.IsSuccessStatusCode)
            return null;

        return await resposta.Content.ReadFromJsonAsync<SessaoDTO>();
    }

    public async Task<SessaoEstadoDTO?> ObterEstadoAsync(int id)
    {
        var resposta = await _http.GetAsync($"api/sessoes/{id}/estado");

        if (!resposta.IsSuccessStatusCode)
            return null;

        return await resposta.Content.ReadFromJsonAsync<SessaoEstadoDTO>();
    }

    public async Task<List<SessaoDTO>> ObterSessoesPorFilmeAsync(int filmeId)
    {
        var resposta = await _http.GetAsync($"api/sessoes/filme/{filmeId}");

        if (!resposta.IsSuccessStatusCode)
            return new();

        return await resposta.Content.ReadFromJsonAsync<List<SessaoDTO>>() ?? new();
    }

    public async Task<List<SessaoDTO>> ObterSessoesPorFestivalAsync(int festivalId)
    {
        var resposta = await _http.GetAsync($"api/sessoes/festival/{festivalId}");

        if (!resposta.IsSuccessStatusCode)
            return new();

        return await resposta.Content.ReadFromJsonAsync<List<SessaoDTO>>() ?? new();
    }
}
