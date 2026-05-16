using System.Net.Http.Headers;
using System.Text.Json;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;

namespace OnlineCinemaFestival.Api.Services;

public class TmdbService : ITmdbService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public TmdbService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IEnumerable<TmdbFilmeDto>> SearchFilmesTmdbAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<TmdbFilmeDto>();

        var result = await GetAsync<TmdbSearchResponse>(
            $"search/movie?query={Uri.EscapeDataString(query)}&language=pt-PT"
        );

        return result?.Results?.Select(FilmeMapper.MapFromTmdbResult)
            ?? Enumerable.Empty<TmdbFilmeDto>();
    }

    public async Task<IEnumerable<TmdbFilmeDto>> GetFilmesIniciaisAsync()
    {
        var result = await GetAsync<TmdbSearchResponse>(
            "movie/popular?language=pt-PT&page=1"
        );

        return result?.Results?.Take(20).Select(FilmeMapper.MapFromTmdbResult)
            ?? Enumerable.Empty<TmdbFilmeDto>();
    }

    public async Task<TmdbFilmeDto?> GetFilmeByTmdbIdAsync(int tmdbId)
    {
        var filmeTmdb = await GetAsync<TmdbMovieDetails>($"movie/{tmdbId}?language=pt-PT");

        if (filmeTmdb == null)
            return null;

        var atores = (await GetAtoresAsync(tmdbId)).ToList();
        var realizador = await GetRealizadorAsync(tmdbId);
        var reviews = (await GetReviewsAsync(tmdbId)).ToList();
        var trailerUrl = await GetTrailerUrlAsync(tmdbId);

        return new TmdbFilmeDto
        {
            TmdbId = filmeTmdb.TmdbId,
            Titulo = filmeTmdb.Titulo,
            TituloOriginal = filmeTmdb.TituloOriginal,
            Sinopse = filmeTmdb.Sinopse,
            DataLancamento = DateTime.TryParse(filmeTmdb.DataLancamento, out var date)
                ? date
                : DateTime.MinValue,
            DuracaoMinutos = filmeTmdb.DuracaoMinutos,
            CapaUrl = !string.IsNullOrWhiteSpace(filmeTmdb.CapaUrl)
                ? $"https://image.tmdb.org/t/p/w500{filmeTmdb.CapaUrl}"
                : string.Empty,
            Classificacao = filmeTmdb.Classificacao?.ToString("0.0"),
            AvaliacaoTmdb = filmeTmdb.Classificacao,
            Generos = filmeTmdb.Genres.Select(g => g.Name).Where(n => !string.IsNullOrWhiteSpace(n)).ToList(),
            Genero = filmeTmdb.Genres.Any()
                ? string.Join(", ", filmeTmdb.Genres.Select(g => g.Name))
                : "Geral",
            TrailerUrl = trailerUrl,
            Realizador = realizador,
            Atores = atores,
            Reviews = reviews,
        };
    }

    public async Task<IEnumerable<string>> GetAtoresAsync(int tmdbId)
    {
        var credits = await GetAsync<TmdbCreditsResponse>($"movie/{tmdbId}/credits?language=pt-PT");

        return credits
                ?.Cast.OrderBy(c => c.Order)
                .Take(10)
                .Select(c => c.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
            ?? Enumerable.Empty<string>();
    }

    public async Task<string?> GetRealizadorAsync(int tmdbId)
    {
        var credits = await GetAsync<TmdbCreditsResponse>($"movie/{tmdbId}/credits?language=pt-PT");

        return credits
            ?.Crew.FirstOrDefault(c => c.Job.Equals("Director", StringComparison.OrdinalIgnoreCase))
            ?.Name;
    }

    public async Task<IEnumerable<TmdbReviewDto>> GetReviewsAsync(int tmdbId)
    {
        var reviews = await GetAsync<TmdbReviewsResponse>($"movie/{tmdbId}/reviews?language=en-US&page=1");

        return reviews
                ?.Results.Take(10)
                .Select(r => new TmdbReviewDto
                {
                    Autor = r.Author,
                    Texto = r.Content,
                    Url = r.Url,
                    CriadaEm = r.CreatedAt,
                    Nota = r.AuthorDetails?.Rating,
                })
            ?? Enumerable.Empty<TmdbReviewDto>();
    }

    public async Task<IEnumerable<TmdbGeneroDto>> GetGenerosAsync()
    {
        var result = await GetAsync<TmdbGenreResponse>("genre/movie/list?language=pt-PT");

        return result
                ?.Genres.Select(g => new TmdbGeneroDto { Id = g.Id, Nome = g.Name })
            ?? Enumerable.Empty<TmdbGeneroDto>();
    }

    public async Task<string?> GetTrailerUrlAsync(int tmdbId)
    {
        var videos = await GetAsync<TmdbVideosResponseDto>($"movie/{tmdbId}/videos?language=pt-PT");

        var trailer = videos
            ?.Results.Where(v =>
                v.Site.Equals("YouTube", StringComparison.OrdinalIgnoreCase)
                && v.Type.Equals("Trailer", StringComparison.OrdinalIgnoreCase)
            )
            .OrderByDescending(v => v.Official)
            .FirstOrDefault();

        if (trailer == null || string.IsNullOrWhiteSpace(trailer.Key))
            return null;

        return $"https://www.youtube.com/embed/{trailer.Key}";
    }

    private async Task<T?> GetAsync<T>(string path)
    {
        var token = _configuration["Tmdb:Token"];
        var baseUrl = _configuration["Tmdb:BaseUrl"];

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("Configuracao TMDB em falta.");

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}{path}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return default;

        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonString, JsonOptions);
    }
}
