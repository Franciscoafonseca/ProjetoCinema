using System.Net.Http.Headers;
using System.Text.Json;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Mappers;

namespace OnlineCinemaFestival.Api.Services;

public class TmdbService : ITmdbService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TmdbService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IEnumerable<TmdbFilmeDto>> SearchFilmesTmdbAsync(string query)
    {
        var token = _configuration["Tmdb:Token"];
        var baseUrl = _configuration["Tmdb:BaseUrl"];

        var url = $"{baseUrl}search/movie?query={Uri.EscapeDataString(query)}&language=pt-PT";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var tmdbResult = JsonSerializer.Deserialize<TmdbSearchResponse>(jsonString, options);

        return tmdbResult?.Results?.Select(FilmeMapper.MapFromTmdbResult) ?? Enumerable.Empty<TmdbFilmeDto>();

    }

    public async Task<TmdbFilmeDto?> GetFilmeByTmdbIdAsync(int tmdbId)
    {
        var token = _configuration["Tmdb:Token"];
        var baseUrl = _configuration["Tmdb:BaseUrl"];

        var url = $"{baseUrl}movie/{tmdbId}?language=pt-PT";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var jsonString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var filmeTmdb = JsonSerializer.Deserialize<TmdbMovieDetails>(jsonString, options);

        if (filmeTmdb == null) return null;

        return new TmdbFilmeDto
        {
            TmdbId = filmeTmdb.TmdbId,
            Titulo = filmeTmdb.Titulo,
            Sinopse = filmeTmdb.Sinopse,
            DataLancamento = DateTime.TryParse(filmeTmdb.DataLancamento, out var date)
                ? date
                : DateTime.MinValue,
            CapaUrl = !string.IsNullOrWhiteSpace(filmeTmdb.CapaUrl)
                ? $"https://image.tmdb.org/t/p/w500{filmeTmdb.CapaUrl}"
                : "",
            Classificacao = filmeTmdb.Classificacao?.ToString("0.0"),
            // 2. Transforma a lista de generos numa string separada por virgulas
            Genero = filmeTmdb.Genres != null && filmeTmdb.Genres.Any()
                ? string.Join(", ", filmeTmdb.Genres.Select(g => g.Name))
                : "Geral"
        };

    }


}