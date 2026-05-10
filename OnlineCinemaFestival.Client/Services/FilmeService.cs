using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class FilmeService
{
    private readonly HttpClient _http;

    public FilmeService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FilmeDto>> GetFilmesAsync(string? genero = null, string? search = null)
    {
        var filmes = await _http.GetFromJsonAsync<List<FilmeDto>>("api/filmes") ?? new();

        if (!string.IsNullOrWhiteSpace(genero))
        {
            filmes = filmes
                .Where(f => string.Equals(f.Genero, genero, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            filmes = filmes
                .Where(f => f.Titulo.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return filmes;
    }

    public async Task<FilmeDto?> GetFilmeByIdAsync(int id)
    {
        var filmes = await _http.GetFromJsonAsync<List<FilmeDto>>("api/filmes") ?? new();

        return filmes.FirstOrDefault(f => f.Id == id);
    }

    public async Task<List<string>> GetGenerosAsync()
    {
        var filmes = await GetFilmesAsync();

        return filmes
            .Where(f => !string.IsNullOrWhiteSpace(f.Genero))
            .Select(f => f.Genero!)
            .Distinct()
            .OrderBy(g => g)
            .ToList();
    }
}
