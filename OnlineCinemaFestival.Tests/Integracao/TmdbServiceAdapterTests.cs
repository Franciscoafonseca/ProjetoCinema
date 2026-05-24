using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Tests.Integracao;

/// <summary>
/// Testa o <see cref="TmdbService"/> (Adapter sobre a API TMDB) com um cliente HTTP falso.
/// Sem chamadas reais à internet — cobre mapping, cache e resiliência a falhas.
/// </summary>
public class TmdbServiceAdapterTests
{
    // ── Pesquisa ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Pesquisar_QueryVazia_RetornaListaVazia()
    {
        var service = CriarServico(new TmdbApiClientFalso());

        var resultado = await service.SearchFilmesTmdbAsync("   ");

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task Pesquisar_ComResultados_MapeiaTitulo()
    {
        var cliente = new TmdbApiClientFalso();
        cliente.RegistarResposta<TmdbSearchResponse>(
            "search/movie",
            new TmdbSearchResponse
            {
                Results = new List<TmdbFilmeResult>
                {
                    new() { TmdbId = 101, Titulo = "Baraka", Classificacao = 8.5 },
                },
            }
        );
        var service = CriarServico(cliente);

        var resultado = (await service.SearchFilmesTmdbAsync("Baraka")).ToList();

        Assert.Single(resultado);
        Assert.Equal(101, resultado[0].TmdbId);
        Assert.Equal("Baraka", resultado[0].Titulo);
    }

    [Fact]
    public async Task Pesquisar_ApiClientLancaExcecao_RetornaListaVazia()
    {
        var cliente = new TmdbApiClientFalso(lancarExcecao: true);
        var service = CriarServico(cliente);

        // Não deve lançar; retorna lista vazia por resiliência
        var resultado = await service.SearchFilmesTmdbAsync("falha");

        Assert.Empty(resultado);
    }

    // ── Cache ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Pesquisar_MesmaQueryDuasVezes_UsaCache()
    {
        var cliente = new TmdbApiClientFalso();
        cliente.RegistarResposta<TmdbSearchResponse>(
            "search/movie",
            new TmdbSearchResponse
            {
                Results = new List<TmdbFilmeResult>
                {
                    new() { TmdbId = 200, Titulo = "Samsara" },
                },
            }
        );
        var service = CriarServico(cliente);

        await service.SearchFilmesTmdbAsync("Samsara");
        await service.SearchFilmesTmdbAsync("Samsara");

        // Deve ter chamado a API apenas 1 vez (cache activo)
        Assert.Equal(1, cliente.ContadorChamadas);
    }

    // ── Géneros ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task ObterGeneros_ComResultados_MapeiaIdENome()
    {
        var cliente = new TmdbApiClientFalso();
        cliente.RegistarResposta<TmdbGenreResponse>(
            "genre/movie/list",
            new TmdbGenreResponse
            {
                Genres = new List<TmdbGenre>
                {
                    new() { Id = 28, Name = "Action" },
                    new() { Id = 18, Name = "Drama" },
                },
            }
        );
        var service = CriarServico(cliente);

        var generos = (await service.ObterGenerosAsync()).ToList();

        Assert.Equal(2, generos.Count);
        Assert.Contains(generos, g => g.Id == 28 && g.Nome == "Action");
        Assert.Contains(generos, g => g.Id == 18 && g.Nome == "Drama");
    }

    [Fact]
    public async Task ObterGeneros_ApiClientLancaExcecao_RetornaListaVazia()
    {
        var service = CriarServico(new TmdbApiClientFalso(lancarExcecao: true));

        var resultado = await service.ObterGenerosAsync();

        Assert.Empty(resultado);
    }

    // ── Detalhes de filme ─────────────────────────────────────────────────────

    [Fact]
    public async Task ObterFilmePorTmdbId_ComTrailerYoutube_IncluiUrlEmbed()
    {
        var cliente = new TmdbApiClientFalso();
        cliente.RegistarResposta<TmdbMovieDetails>(
            "movie/101",
            new TmdbMovieDetails
            {
                TmdbId = 101,
                Titulo = "Koyaanisqatsi",
                Genres = new List<TmdbGenre> { new() { Id = 99, Name = "Documentary" } },
                Credits = new TmdbCreditsResponse(),
                Reviews = new TmdbReviewsResponse(),
                Videos = new TmdbVideosResponseDTO
                {
                    Results = new List<TmdbVideoDTO>
                    {
                        new()
                        {
                            Key = "abc123",
                            Site = "YouTube",
                            Type = "Trailer",
                            Official = true,
                            Name = "Official Trailer",
                        },
                    },
                },
            }
        );
        var service = CriarServico(cliente);

        var filme = await service.ObterFilmePorTmdbIdAsync(101);

        Assert.NotNull(filme);
        Assert.Contains("abc123", filme!.TrailerUrl);
        Assert.Contains("youtube.com/embed/", filme.TrailerUrl);
    }

    [Fact]
    public async Task ObterFilmePorTmdbId_FilmeInexistente_RetornaNull()
    {
        var service = CriarServico(new TmdbApiClientFalso()); // sem dados registados

        var resultado = await service.ObterFilmePorTmdbIdAsync(999);

        Assert.Null(resultado);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static TmdbService CriarServico(ITmdbApiClient cliente)
    {
        var cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        return new TmdbService(cliente, cache);
    }

    // ── Cliente HTTP falso ────────────────────────────────────────────────────

    private sealed class TmdbApiClientFalso : ITmdbApiClient
    {
        private readonly bool _lancarExcecao;
        private readonly Dictionary<string, object?> _respostas = new();
        public int ContadorChamadas { get; private set; }

        public TmdbApiClientFalso(bool lancarExcecao = false)
        {
            _lancarExcecao = lancarExcecao;
        }

        public void RegistarResposta<T>(string prefixoPath, T resposta)
        {
            _respostas[prefixoPath] = resposta;
        }

        public Task<T?> GetAsync<T>(string path)
        {
            ContadorChamadas++;

            if (_lancarExcecao)
                throw new HttpRequestException("Erro simulado de rede.");

            foreach (var (prefixo, valor) in _respostas)
            {
                if (path.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase) && valor is T tipado)
                    return Task.FromResult<T?>(tipado);
            }

            return Task.FromResult<T?>(default);
        }
    }
}
