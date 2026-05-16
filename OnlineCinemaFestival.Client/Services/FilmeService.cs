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

    public async Task<List<FilmeDto>> GetFilmesAsync(
        string? genero = null,
        string? pesquisa = null,
        int? ordenarPor = null,
        bool descendente = false
    )
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

        var url =
            parametros.Count > 0 ? "api/catalogo?" + string.Join("&", parametros) : "api/catalogo";

        return await _http.GetFromJsonAsync<List<FilmeDto>>(url) ?? new();
    }

    public async Task<FilmeDto?> GetFilmeByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<FilmeDto>($"api/filmes/{id}");
    }

    public async Task<List<FilmeDto>> GetFilmesIniciaisTmdbAsync()
    {
        return await _http.GetFromJsonAsync<List<FilmeDto>>("api/tmdb/filmes-iniciais")
            ?? new();
    }

    public async Task<List<FilmeDto>> PesquisarTmdbAsync(string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return new();

        return await _http.GetFromJsonAsync<List<FilmeDto>>(
                $"api/tmdb/pesquisar?termo={Uri.EscapeDataString(termo)}"
            )
            ?? new();
    }

    public async Task<FilmeDto> ImportarTmdbAsync(int tmdbId)
    {
        var resposta = await _http.PostAsync($"api/filmes/importar-tmdb/{tmdbId}", null);

        if (!resposta.IsSuccessStatusCode)
        {
            var conteudo = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(conteudo)
                    ? "Nao foi possivel importar o filme."
                    : conteudo.Trim('"')
            );
        }

        return await resposta.Content.ReadFromJsonAsync<FilmeDto>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<List<string>> GetGenerosAsync()
    {
        var filmes = await GetFilmesAsync();

        return filmes
            .Where(f => !string.IsNullOrWhiteSpace(f.Genero))
            .SelectMany(f => f.Genero!.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(g => g.Trim())
            .Where(g => g.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g)
            .ToList();
    }
}
