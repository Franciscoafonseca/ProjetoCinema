using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class TmdbService
{
    private readonly HttpClient _http;

    public TmdbService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FilmeDto>> GetFilmesIniciaisAsync()
    {
        return await _http.GetFromJsonAsync<List<FilmeDto>>("api/tmdb/filmes-iniciais")
            ?? new();
    }

    public async Task<List<FilmeDto>> PesquisarAsync(string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return new();

        return await _http.GetFromJsonAsync<List<FilmeDto>>(
                $"api/tmdb/pesquisar?termo={Uri.EscapeDataString(termo)}"
            )
            ?? new();
    }
}
