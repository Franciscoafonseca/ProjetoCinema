using Microsoft.Extensions.Caching.Memory;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Services.Catalogo;
using OnlineCinemaFestival.Client.Services;
using OnlineCinemaFestival.Tests.Support.Builders;

namespace OnlineCinemaFestival.Tests.Recomendacoes;

public class CatalogoTests
{
    [Fact]
    public void Reviews_PorDefeito_Mostram10()
    {
        var reviews = Enumerable.Range(1, 12).Select(i => new OnlineCinemaFestival.Client.Models.AvaliacaoDTO
        {
            Id = i,
            Data = new DateTime(2026, 5, i),
        });

        var visiveis = ReviewListagemPolicy.ObterVisiveis(reviews, ReviewListagemPolicy.QuantidadeInicial);

        Assert.Equal(10, visiveis.Count);
        Assert.Equal(12, visiveis[0].Id);
    }

    [Fact]
    public void VerMaisReviews_CarregaProximaQuantidade()
    {
        var proximaQuantidade = ReviewListagemPolicy.ProximaQuantidade(25, 10);

        Assert.Equal(20, proximaQuantidade);
    }

    [Fact]
    public async Task Catalogo_FiltroMultiGenero_DevolveQualquerEPriorizaMaiorCorrespondencia()
    {
        var acao = new Genero { Name = "Acao" };
        var drama = new Genero { Name = "Drama" };
        var comedia = new Genero { Name = "Comedia" };
        var filmes = new List<Filme>
        {
            new FilmeBuilder().ComId(1).ComTitulo("So Acao").ComGenero(acao).Build(),
            new FilmeBuilder().ComId(2).ComTitulo("Acao e Drama").ComGenero(acao, drama).Build(),
            new FilmeBuilder().ComId(3).ComTitulo("So Comedia").ComGenero(comedia).Build(),
        };
        var service = CriarCatalogoService(filmes);

        var resultado = (await service.ObterCatalogoAsync(new CatalogoQueryDTO
        {
            Genero = "Acao,Drama",
            OrdenarPor = CatalogoOrdenacao.Titulo,
        })).ToList();

        Assert.Equal(new[] { "Acao e Drama", "So Acao" }, resultado.Select(f => f.Titulo));
    }

    [Fact]
    public async Task Catalogo_OrdenacoesDisponiveis_DevolvemFilmeCorretoNaPrimeiraPosicao()
    {
        var filmes = new List<Filme>
        {
            FilmeCatalogo(1, "Beta", new DateTime(2024, 1, 1), avaliacaoTmdb: 7, popularidade: 1),
            FilmeCatalogo(2, "Alfa", new DateTime(2025, 1, 1), avaliacaoTmdb: 9, popularidade: 3),
        };
        var service = CriarCatalogoService(filmes);

        var popularidade = await PrimeiroTitulo(service, CatalogoOrdenacao.Popularidade, true);
        var classificacao = await PrimeiroTitulo(service, CatalogoOrdenacao.Classificacao, true);
        var titulo = await PrimeiroTitulo(service, CatalogoOrdenacao.Titulo, false);
        var lancamento = await PrimeiroTitulo(service, CatalogoOrdenacao.DataLancamento, true);

        Assert.Equal("Alfa", popularidade);
        Assert.Equal("Alfa", classificacao);
        Assert.Equal("Alfa", titulo);
        Assert.Equal("Alfa", lancamento);
    }

    [Fact]
    public async Task TmdbService_QuandoIndisponivel_DevolveVazioSemCrash()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OnlineCinemaFestival.Api.Services.TmdbService(new TmdbApiClientIndisponivel(), cache);

        var filmes = await service.ObterFilmesIniciaisAsync();
        var pesquisa = await service.SearchFilmesTmdbAsync("teste");
        var detalhe = await service.ObterFilmePorTmdbIdAsync(123);

        Assert.Empty(filmes);
        Assert.Empty(pesquisa);
        Assert.Null(detalhe);
    }

    private static async Task<string> PrimeiroTitulo(
        CatalogoService service,
        CatalogoOrdenacao ordenacao,
        bool descendente
    )
    {
        var resultado = await service.ObterCatalogoAsync(new CatalogoQueryDTO
        {
            OrdenarPor = ordenacao,
            Descendente = descendente,
        });

        return resultado.First().Titulo;
    }

    private static CatalogoService CriarCatalogoService(List<Filme> filmes)
    {
        var strategies = new ICatalogoOrdenacaoStrategy[]
        {
            new OrdenarPorTituloStrategy(),
            new OrdenarPorPopularidadeStrategy(),
            new OrdenarPorClassificacaoStrategy(),
            new OrdenarPorDataLancamentoStrategy(),
            new OrdenarPorVisualizacoesStrategy(),
            new OrdenarPorFestivalStrategy(),
        };

        return new CatalogoService(
            new FilmeRepositoryFalso(filmes),
            new FestivalRepositoryFalso(),
            new FestivalFilmeRepositoryFalso(filmes),
            new CatalogoOrdenacaoStrategyFactory(strategies)
        );
    }

    private static Filme FilmeCatalogo(int id, string titulo, DateTime lancamento, double avaliacaoTmdb, int popularidade)
    {
        var pontuacoes = Enumerable.Repeat(8, popularidade).ToArray();
        var filme = new FilmeBuilder()
            .ComId(id)
            .ComTitulo(titulo)
            .LancadoEm(lancamento)
            .ComAvaliacaoTmdb(avaliacaoTmdb)
            .ComAvaliacoes(pontuacoes)
            .Build();
        return filme;
    }

    private sealed class FilmeRepositoryFalso : IFilmeRepository
    {
        private readonly List<Filme> _filmes;
        public FilmeRepositoryFalso(List<Filme> filmes) { _filmes = filmes; }

        public Task<IEnumerable<Filme>> ObterTodosAsync() => Task.FromResult<IEnumerable<Filme>>(_filmes);
        public Task<Filme?> ObterPorIdAsync(int id) => Task.FromResult(_filmes.FirstOrDefault(f => f.Id == id));
        public Task<Filme?> ObterDetalhePorIdAsync(int id) => ObterPorIdAsync(id);
        public Task<Filme?> ObterPorTmdbIdAsync(int tmdbId) => Task.FromResult(_filmes.FirstOrDefault(f => f.TmdbId == tmdbId));
        public Task<List<Filme>> ObterPrincipaisAsync(int quantidade) => Task.FromResult(_filmes.Take(quantidade).ToList());
        public Task<List<Festival>> ObterFestivaisDoFilmeAsync(int filmeId) => Task.FromResult(new List<Festival>());
        public Task<List<Sessao>> ObterSessoesDoFilmeAsync(int filmeId) => Task.FromResult(new List<Sessao>());
        public Task<Genero> ObterOuCriarGeneroAsync(string nome) => Task.FromResult(new Genero { Name = nome });
        public Task<Pessoa> ObterOuCriarPessoaAsync(int? tmdbPessoaId, string nome, string? imagemUrl) =>
            Task.FromResult(new Pessoa { TmdbPessoaId = tmdbPessoaId, Nome = nome, ImagemUrl = imagemUrl });
        public Task<bool> UtilizadorViuFilmeAsync(int utilizadorId, int filmeId) => Task.FromResult(false);
        public Task<Avaliacao?> ObterAvaliacaoAsync(int utilizadorId, int filmeId) => Task.FromResult<Avaliacao?>(null);
        public Task AddAvaliacaoAsync(Avaliacao avaliacao) => Task.CompletedTask;
        public Task AddAsync(Filme filme) { _filmes.Add(filme); return Task.CompletedTask; }
        public void AtualizarVideo(Filme filme, string? provider, string? key, string? url) { }
        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class FestivalRepositoryFalso : IFestivalRepository
    {
        public Task<IEnumerable<Festival>> ObterTodosAsync() => Task.FromResult(Enumerable.Empty<Festival>());
        public Task<Festival?> ObterPorIdAsync(int id) => Task.FromResult<Festival?>(new Festival { Id = id });
        public Task<Festival?> ObterDetalhePorIdAsync(int id) => ObterPorIdAsync(id);
        public Task AddAsync(Festival festival) => Task.CompletedTask;
        public void Remove(Festival festival) { }
        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class FestivalFilmeRepositoryFalso : IFestivalFilmeRepository
    {
        private readonly List<Filme> _filmes;
        public FestivalFilmeRepositoryFalso(List<Filme> filmes) { _filmes = filmes; }

        public Task<bool> ExisteAsync(int festivalId, int filmeId) => Task.FromResult(false);
        public Task<FestivalFilme?> ObterAsync(int festivalId, int filmeId) => Task.FromResult<FestivalFilme?>(null);
        public Task AdicionarAsync(FestivalFilme festivalFilme) => Task.CompletedTask;
        public void Remove(FestivalFilme festivalFilme) { }
        public Task<IEnumerable<Filme>> ObterFilmesPorFestivalIdAsync(int festivalId) => Task.FromResult<IEnumerable<Filme>>(_filmes);
        public Task<IEnumerable<FestivalFilme>> ObterAssociacoesPorFestivalIdAsync(int festivalId) =>
            Task.FromResult(Enumerable.Empty<FestivalFilme>());
        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class TmdbApiClientIndisponivel : ITmdbApiClient
    {
        public Task<T?> GetAsync<T>(string path) => throw new HttpRequestException("TMDB indisponivel");
    }
}
