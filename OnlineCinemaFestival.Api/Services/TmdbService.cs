using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;

namespace OnlineCinemaFestival.Api.Services;

public class TmdbService : ITmdbService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private const string YouTubeProvider = "YouTube";
    private static readonly TimeSpan SearchCacheDuration = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan PopularCacheDuration = TimeSpan.FromMinutes(15);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public TmdbService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<IEnumerable<TmdbFilmeDTO>> SearchFilmesTmdbAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<TmdbFilmeDTO>();

        var cacheKey = $"tmdb:search:{query.Trim().ToLowerInvariant()}";

        if (_cache.TryGetValue(cacheKey, out List<TmdbFilmeDTO>? cached) && cached != null)
            return cached;

        var result = await GetAsync<TmdbSearchResponse>(
            $"search/movie?query={Uri.EscapeDataString(query)}&language=pt-PT"
        );

        var filmes =
            result?.Results?.Select(FilmeMapper.MapFromTmdbResult).ToList() ?? new List<TmdbFilmeDTO>();

        _cache.Set(cacheKey, filmes, SearchCacheDuration);
        return filmes;
    }

    public async Task<IEnumerable<TmdbFilmeDTO>> ObterFilmesIniciaisAsync()
    {
        const string cacheKey = "tmdb:popular:pt-pt:1";

        if (_cache.TryGetValue(cacheKey, out List<TmdbFilmeDTO>? cached) && cached != null)
            return cached;

        var result = await GetAsync<TmdbSearchResponse>(
            "movie/popular?language=pt-PT&page=1"
        );

        var filmes =
            result?.Results?.Take(20).Select(FilmeMapper.MapFromTmdbResult).ToList()
            ?? new List<TmdbFilmeDTO>();

        _cache.Set(cacheKey, filmes, PopularCacheDuration);
        return filmes;
    }

    public async Task<TmdbFilmeDTO?> ObterFilmePorTmdbIdAsync(int tmdbId)
    {
        var filmeTmdb = await GetAsync<TmdbMovieDetails>(
            $"movie/{tmdbId}?language=pt-PT&append_to_response=videos,credits,reviews"
        );

        if (filmeTmdb == null)
            return null;

        var atoresDetalhes = MapAtores(filmeTmdb.Credits).ToList();
        var realizadorDetalhe = MapPessoaCrew(filmeTmdb.Credits, "Director");
        var produtorDetalhe = MapPessoaCrew(filmeTmdb.Credits, "Producer");
        var video = SelecionarTrailerPrincipal(filmeTmdb.Videos.Results);
        var videoUrl = CriarVideoUrl(video);
        var duracaoVideoSegundos = await ObterDuracaoVideoSegundosAsync(video);

        return new TmdbFilmeDTO
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
            TrailerUrl = videoUrl,
            VideoProvider = video?.Site,
            VideoKey = video?.Key,
            VideoUrl = videoUrl,
            DuracaoVideoSegundos = duracaoVideoSegundos,
            Realizador = realizadorDetalhe?.Nome,
            Atores = atoresDetalhes.Select(a => a.Nome).ToList(),
            AtoresDetalhes = atoresDetalhes,
            RealizadorDetalhe = realizadorDetalhe,
            ProdutorDetalhe = produtorDetalhe,
            Reviews = MapReviews(filmeTmdb.Reviews).ToList(),
        };
    }

    public async Task<IEnumerable<string>> ObterAtoresAsync(int tmdbId)
    {
        var credits = await GetAsync<TmdbCreditsResponse>($"movie/{tmdbId}/credits?language=pt-PT");

        return credits
                ?.Cast.OrderBy(c => c.Order)
                .Take(10)
                .Select(c => c.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
            ?? Enumerable.Empty<string>();
    }

    public async Task<string?> ObterRealizadorAsync(int tmdbId)
    {
        var credits = await GetAsync<TmdbCreditsResponse>($"movie/{tmdbId}/credits?language=pt-PT");

        return credits
            ?.Crew.FirstOrDefault(c => c.Job.Equals("Director", StringComparison.OrdinalIgnoreCase))
            ?.Name;
    }

    public async Task<IEnumerable<TmdbReviewDTO>> ObterAvaliacoesExternasAsync(int tmdbId)
    {
        var reviews = await GetAsync<TmdbReviewsResponse>($"movie/{tmdbId}/reviews?language=en-US&page=1");

        return reviews
                ?.Results.Take(10)
                .Select(r => new TmdbReviewDTO
                {
                    Autor = r.Author,
                    Texto = r.Content,
                    Url = r.Url,
                    CriadaEm = r.CreatedAt,
                    Nota = r.AuthorDetails?.Rating,
                })
            ?? Enumerable.Empty<TmdbReviewDTO>();
    }

    public async Task<IEnumerable<TmdbGeneroDTO>> ObterGenerosAsync()
    {
        var result = await GetAsync<TmdbGenreResponse>("genre/movie/list?language=pt-PT");

        return result
                ?.Genres.Select(g => new TmdbGeneroDTO { Id = g.Id, Nome = g.Name })
            ?? Enumerable.Empty<TmdbGeneroDTO>();
    }

    public async Task<string?> ObterTrailerUrlAsync(int tmdbId)
    {
        var videos = await GetAsync<TmdbVideosResponseDTO>($"movie/{tmdbId}/videos?language=pt-PT");

        return CriarVideoUrl(SelecionarTrailerPrincipal(videos?.Results ?? new List<TmdbVideoDTO>()));
    }

    public int? ConverterDuracaoIso8601ParaSegundos(string? duracaoIso8601)
    {
        if (string.IsNullOrWhiteSpace(duracaoIso8601))
            return null;

        var match = Regex.Match(
            duracaoIso8601.Trim(),
            @"^P(?:(?<days>\d+)D)?(?:T(?:(?<hours>\d+)H)?(?:(?<minutes>\d+)M)?(?:(?<seconds>\d+)S)?)?$",
            RegexOptions.IgnoreCase
        );

        if (!match.Success)
            return null;

        var dias = Valor(match, "days");
        var horas = Valor(match, "hours");
        var minutos = Valor(match, "minutes");
        var segundos = Valor(match, "seconds");

        return (dias * 24 * 60 * 60) + (horas * 60 * 60) + (minutos * 60) + segundos;
    }

    private async Task<int?> ObterDuracaoVideoSegundosAsync(TmdbVideoDTO? video)
    {
        if (video == null || string.IsNullOrWhiteSpace(video.Key))
            return null;

        if (!video.Site.Equals(YouTubeProvider, StringComparison.OrdinalIgnoreCase))
            return null;

        var apiKey = _configuration["YouTube:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            return null;

        var baseUrl = _configuration["YouTube:BaseUrl"] ?? "https://www.googleapis.com/youtube/v3/";
        var url =
            $"{baseUrl.TrimEnd('/')}/videos?part=contentDetails&id={Uri.EscapeDataString(video.Key)}&key={Uri.EscapeDataString(apiKey)}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        var jsonString = await response.Content.ReadAsStringAsync();
        var youtubeResponse = JsonSerializer.Deserialize<YouTubeVideosResponse>(jsonString, JsonOptions);
        var duration = youtubeResponse?.Items.FirstOrDefault()?.ContentDetails.Duration;

        return ConverterDuracaoIso8601ParaSegundos(duration);
    }

    private static IEnumerable<TmdbPessoaDTO> MapAtores(TmdbCreditsResponse credits)
    {
        return credits
            .Cast.OrderBy(c => c.Order)
            .Take(10)
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .Select(c => new TmdbPessoaDTO
            {
                TmdbPessoaId = c.TmdbPessoaId > 0 ? c.TmdbPessoaId : null,
                Nome = c.Name,
                ImagemUrl = CriarImagemPerfilUrl(c.ProfilePath),
                Personagem = c.Character,
                Ordem = c.Order,
            });
    }

    private static TmdbPessoaDTO? MapPessoaCrew(TmdbCreditsResponse credits, string job)
    {
        var pessoa = credits.Crew.FirstOrDefault(c =>
            c.Job.Equals(job, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(c.Name)
        );

        if (pessoa == null)
            return null;

        return new TmdbPessoaDTO
        {
            TmdbPessoaId = pessoa.TmdbPessoaId > 0 ? pessoa.TmdbPessoaId : null,
            Nome = pessoa.Name,
            ImagemUrl = CriarImagemPerfilUrl(pessoa.ProfilePath),
        };
    }

    private static IEnumerable<TmdbReviewDTO> MapReviews(TmdbReviewsResponse reviews)
    {
        return reviews.Results.Take(10).Select(r => new TmdbReviewDTO
        {
            Autor = r.Author,
            Texto = r.Content,
            Url = r.Url,
            CriadaEm = r.CreatedAt,
            Nota = r.AuthorDetails?.Rating,
        });
    }

    private static TmdbVideoDTO? SelecionarTrailerPrincipal(IEnumerable<TmdbVideoDTO> videos)
    {
        return videos
            .Where(v =>
                v.Site.Equals(YouTubeProvider, StringComparison.OrdinalIgnoreCase)
                && v.Type.Equals("Trailer", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(v.Key)
            )
            .OrderByDescending(v => v.Official)
            .ThenBy(v => v.Name.Contains("trailer", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .FirstOrDefault();
    }

    private static string? CriarVideoUrl(TmdbVideoDTO? video)
    {
        if (video == null || string.IsNullOrWhiteSpace(video.Key))
            return null;

        return video.Site.Equals(YouTubeProvider, StringComparison.OrdinalIgnoreCase)
            ? $"https://www.youtube.com/embed/{video.Key}"
            : null;
    }

    private static string? CriarImagemPerfilUrl(string? profilePath)
    {
        return string.IsNullOrWhiteSpace(profilePath)
            ? null
            : $"https://image.tmdb.org/t/p/w185{profilePath}";
    }

    private static int Valor(Match match, string groupName)
    {
        return match.Groups[groupName].Success
            && int.TryParse(match.Groups[groupName].Value, out var value)
            ? value
            : 0;
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
