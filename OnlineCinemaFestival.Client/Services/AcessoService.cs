using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class AcessoService
{
    private readonly HttpClient _http;

    public AcessoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<AcessoDto>> GetAcessosAsync()
    {
        return await _http.GetFromJsonAsync<List<AcessoDto>>("api/acessos") ?? new();
    }

    public async Task<List<TipoAcessoDto>> GetTiposAcessoAsync()
    {
        return await _http.GetFromJsonAsync<List<TipoAcessoDto>>("api/acessos/tipos") ?? new();
    }
}
