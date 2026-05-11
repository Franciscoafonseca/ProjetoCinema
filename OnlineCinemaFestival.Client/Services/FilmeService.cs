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

    public async Task<List<FilmeDto>> GetFilmesAsync(string? genero = null, string? pesquisa = null, int? ordenarPor = null, bool descendente = false)
    {
        var parametros = new List<string>();

        if (!string.IsNullOrWhiteSpace(genero))
            parametros.Add($"genero={Uri.EscapeDataString(genero)}");

        if (!string.IsNullOrWhiteSpace(pesquisa))
            parametros.Add($"pesquisa={Uri.EscapeDataString(pesquisa)}");

        if (ordenarPor.HasValue)
            parametros.Add($"ordenarPor={ordenarPor.Value}");

        if (descendente)
            parametros.Add("descendente=true");

        var url = parametros.Count > 0
            ? "api/catalogo?" + string.Join("&", parametros)
            : "api/catalogo";

        return await _http.GetFromJsonAsync<List<FilmeDto>>(url) ?? new();
    }

    public async Task<FilmeDto?> GetFilmeByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<FilmeDto>($"api/catalogo/filmes/{id}");
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
