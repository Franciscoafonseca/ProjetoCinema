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

    public async Task<List<FilmeDTO>> ObterFilmesAsync(
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

        return await _http.GetFromJsonAsync<List<FilmeDTO>>(url) ?? new();
    }

    public async Task<FilmeDTO?> ObterFilmePorIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<FilmeDTO>($"api/filmes/{id}");
    }

    public async Task<List<FilmeDTO>> ObterFilmesIniciaisTmdbAsync()
    {
        return await _http.GetFromJsonAsync<List<FilmeDTO>>("api/tmdb/filmes-iniciais")
            ?? new();
    }

    public async Task<List<FilmeDTO>> PesquisarTmdbAsync(string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return new();

        return await _http.GetFromJsonAsync<List<FilmeDTO>>(
                $"api/tmdb/pesquisar?termo={Uri.EscapeDataString(termo)}"
            )
            ?? new();
    }

    public async Task<FilmeDTO> ImportarTmdbAsync(int tmdbId)
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

        return await resposta.Content.ReadFromJsonAsync<FilmeDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<List<string>> ObterGenerosAsync()
    {
        var filmes = await ObterFilmesAsync();

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
