using Microsoft.Extensions.Caching.Memory;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Mapping;

namespace OnlineCinemaFestival.Api.Services;

public class TmdbService : ITmdbService
{
    private readonly ITmdbApiClient _apiClient;
    private readonly IMemoryCache _cache;
    private const string YouTubeProvider = "YouTube";
    private static readonly TimeSpan SearchCacheDuration = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan PopularCacheDuration = TimeSpan.FromMinutes(15);

    public TmdbService(ITmdbApiClient apiClient, IMemoryCache cache)
    {
        _apiClient = apiClient;
        _cache = cache;
    }

    public async Task<IEnumerable<TmdbFilmeDTO>> SearchFilmesTmdbAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<TmdbFilmeDTO>();

        var cacheKey = $"tmdb:search:{query.Trim().ToLowerInvariant()}";

        if (_cache.TryGetValue(cacheKey, out List<TmdbFilmeDTO>? cached) && cached != null)
            return cached;

        try
        {
            var result = await _apiClient.GetAsync<TmdbSearchResponse>(
                $"search/movie?query={Uri.EscapeDataString(query)}&language=pt-PT"
            );

            var filmes =
                result?.Results?.Select(FilmeMapper.MapFromTmdbResult).ToList()
                ?? new List<TmdbFilmeDTO>();

            _cache.Set(cacheKey, filmes, SearchCacheDuration);
            return filmes;
        }
        catch
        {
            return Enumerable.Empty<TmdbFilmeDTO>();
        }
    }

    public async Task<IEnumerable<TmdbFilmeDTO>> ObterFilmesIniciaisAsync()
    {
        const string cacheKey = "tmdb:initial:pt-pt:random";

        if (_cache.TryGetValue(cacheKey, out List<TmdbFilmeDTO>? cached) && cached != null)
            return cached;

        try
        {
            var result = await ObterPopularesPreferindoSkyShowtimeAsync()
                ?? await _apiClient.GetAsync<TmdbSearchResponse>(
                    $"movie/popular?language=pt-PT&page={Random.Shared.Next(1, 6)}"
                );

            var filmes =
                result
                    ?.Results?
                    .Where(f => f.TmdbId > 0)
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(20)
                    .Select(FilmeMapper.MapFromTmdbResult)
                    .ToList()
                ?? new List<TmdbFilmeDTO>();

            _cache.Set(cacheKey, filmes, PopularCacheDuration);
            return filmes;
        }
        catch
        {
            return Enumerable.Empty<TmdbFilmeDTO>();
        }
    }

    private async Task<TmdbSearchResponse?> ObterPopularesPreferindoSkyShowtimeAsync()
    {
        try
        {
            var providers = await _apiClient.GetAsync<TmdbWatchProviderResponse>(
                "watch/providers/movie?language=pt-PT&watch_region=PT"
            );

            var providerIds =
                providers
                    ?.Results.Where(p =>
                        p.ProviderName.Contains("Sky", StringComparison.OrdinalIgnoreCase)
                        || p.ProviderName.Contains("Showtime", StringComparison.OrdinalIgnoreCase)
                    )
                    .Select(p => p.ProviderId)
                    .Distinct()
                    .ToList() ?? new List<int>();

            if (providerIds.Count == 0)
                return null;

            var result = await _apiClient.GetAsync<TmdbSearchResponse>(
                "discover/movie?language=pt-PT"
                    + "&watch_region=PT"
                    + $"&with_watch_providers={string.Join('|', providerIds)}"
                    + $"&page={Random.Shared.Next(1, 4)}"
                    + "&sort_by=popularity.desc"
            );

            return result?.Results.Count > 0 ? result : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<TmdbFilmeDTO?> ObterFilmePorTmdbIdAsync(int tmdbId)
    {
        try
        {
            var filmeTmdb = await _apiClient.GetAsync<TmdbMovieDetails>(
                $"movie/{tmdbId}?language=pt-PT&append_to_response=videos,credits,reviews"
            );

            if (filmeTmdb == null)
                return null;

            var atoresDetalhes = MapAtores(filmeTmdb.Credits).ToList();
            var realizadorDetalhe = MapPessoaCrew(filmeTmdb.Credits, "Director");
            var produtorDetalhe = MapPessoaCrew(filmeTmdb.Credits, "Producer");
            var video = SelecionarTrailerPrincipal(filmeTmdb.Videos.Results);
            var videoUrl = CriarVideoUrl(video);

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
                Realizador = realizadorDetalhe?.Nome,
                Atores = atoresDetalhes.Select(a => a.Nome).ToList(),
                AtoresDetalhes = atoresDetalhes,
                RealizadorDetalhe = realizadorDetalhe,
                ProdutorDetalhe = produtorDetalhe,
                Reviews = MapReviews(filmeTmdb.Reviews).ToList(),
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<string>> ObterAtoresAsync(int tmdbId)
    {
        try
        {
            var credits = await _apiClient.GetAsync<TmdbCreditsResponse>($"movie/{tmdbId}/credits?language=pt-PT");

            return credits
                    ?.Cast.OrderBy(c => c.Order)
                    .Take(10)
                    .Select(c => c.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                ?? Enumerable.Empty<string>();
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }

    public async Task<string?> ObterRealizadorAsync(int tmdbId)
    {
        try
        {
            var credits = await _apiClient.GetAsync<TmdbCreditsResponse>($"movie/{tmdbId}/credits?language=pt-PT");

            return credits
                ?.Crew.FirstOrDefault(c => c.Job.Equals("Director", StringComparison.OrdinalIgnoreCase))
                ?.Name;
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<TmdbReviewDTO>> ObterAvaliacoesExternasAsync(int tmdbId)
    {
        try
        {
            var reviews = await _apiClient.GetAsync<TmdbReviewsResponse>($"movie/{tmdbId}/reviews?language=en-US&page=1");

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
        catch
        {
            return Enumerable.Empty<TmdbReviewDTO>();
        }
    }

    public async Task<IEnumerable<TmdbGeneroDTO>> ObterGenerosAsync()
    {
        try
        {
            var result = await _apiClient.GetAsync<TmdbGenreResponse>("genre/movie/list?language=pt-PT");

            return result
                    ?.Genres.Select(g => new TmdbGeneroDTO { Id = g.Id, Nome = g.Name })
                ?? Enumerable.Empty<TmdbGeneroDTO>();
        }
        catch
        {
            return Enumerable.Empty<TmdbGeneroDTO>();
        }
    }

    public async Task<string?> ObterTrailerUrlAsync(int tmdbId)
    {
        try
        {
            var videos = await _apiClient.GetAsync<TmdbVideosResponseDTO>($"movie/{tmdbId}/videos?language=pt-PT");

            return CriarVideoUrl(SelecionarTrailerPrincipal(videos?.Results ?? new List<TmdbVideoDTO>()));
        }
        catch
        {
            return null;
        }
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

}
