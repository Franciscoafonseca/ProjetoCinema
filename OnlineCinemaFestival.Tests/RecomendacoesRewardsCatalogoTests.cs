using Microsoft.Extensions.Caching.Memory;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Services.Catalogo;
using OnlineCinemaFestival.Client.Services;
using OnlineCinemaFestival.Tests.Support;

namespace OnlineCinemaFestival.Tests;

public class RecomendacoesRewardsCatalogoTests
{
    [Fact]
    public async Task Recomendacao_por_genero_usa_preferencias_do_utilizador()
    {
        var drama = new Genero { Id = 1, Name = "Drama" };
        var filmes = new List<Filme>
        {
            FilmeComGeneros(1, "Drama Forte", drama),
            FilmeComGeneros(2, "Comedia Leve", new Genero { Id = 2, Name = "Comedia" }),
        };
        var utilizador = new Utilizador
        {
            Id = 7,
            GenerosFavoritos =
            {
                new UtilizadorGeneroFavorito { Genero = drama },
            },
        };
        var service = CriarRecomendacaoService(
            filmes,
            utilizador,
            new RecomendacaoPorGeneroStrategy()
        );

        var resultado = await service.ObterRecomendacoesAsync(utilizador.Id);

        Assert.Single(resultado);
        Assert.Equal("Drama Forte", resultado[0].Filme.Titulo);
        Assert.Contains("Genero", resultado[0].Motivo);
    }

    [Fact]
    public async Task Recomendacao_por_popularidade_ordena_por_visualizacoes()
    {
        var filmes = new List<Filme>
        {
            FilmeComVisualizacoes(1, "Pouco visto", 1),
            FilmeComVisualizacoes(2, "Muito visto", 4),
        };
        var service = CriarRecomendacaoService(
            filmes,
            null,
            new RecomendacaoPorPopularidadeStrategy()
        );

        var resultado = await service.ObterRecomendacoesAsync(1);

        Assert.Equal("Muito visto", resultado[0].Filme.Titulo);
        Assert.Contains("Popular", resultado[0].Motivo);
    }

    [Fact]
    public async Task Recomendacao_por_avaliacao_ordena_por_media()
    {
        var filmes = new List<Filme>
        {
            FilmeComAvaliacoes(1, "Medio", 6, 7),
            FilmeComAvaliacoes(2, "Excelente", 9, 10),
        };
        var service = CriarRecomendacaoService(
            filmes,
            null,
            new RecomendacaoPorAvaliacaoStrategy()
        );

        var resultado = await service.ObterRecomendacoesAsync(1);

        Assert.Equal("Excelente", resultado[0].Filme.Titulo);
        Assert.Contains("avaliado", resultado[0].Motivo);
    }

    [Fact]
    public async Task Recomendacao_sem_preferencias_usa_fallback_do_catalogo()
    {
        var filmes = new List<Filme>
        {
            FilmeComVisualizacoes(1, "Catalogo A", 0),
            FilmeComVisualizacoes(2, "Catalogo B", 2),
        };
        var service = CriarRecomendacaoService(filmes, null);

        var resultado = await service.ObterRecomendacoesAsync(1);

        Assert.Equal(2, resultado.Count);
        Assert.Equal("Catalogo B", resultado[0].Filme.Titulo);
        Assert.All(resultado, r => Assert.Contains("Sugestao geral", r.Motivo));
    }

    [Fact]
    public async Task Recomendacoes_nao_devolvem_duplicados()
    {
        var filme = FilmeComAvaliacoes(1, "Mesmo Filme", 9);
        filme.Visualizacoes.Add(new Visualizacao { Id = 1, FilmeId = filme.Id });
        filme.Visualizacoes.Add(new Visualizacao { Id = 2, FilmeId = filme.Id });
        var service = CriarRecomendacaoService(
            new List<Filme> { filme },
            null,
            new RecomendacaoPorPopularidadeStrategy(),
            new RecomendacaoPorAvaliacaoStrategy()
        );

        var resultado = await service.ObterRecomendacoesAsync(1);

        Assert.Single(resultado);
        Assert.Contains("Popular", resultado[0].Motivo);
        Assert.Contains("avaliado", resultado[0].Motivo);
    }

    [Fact]
    public async Task Rewards_atribui_pontos_por_acao()
    {
        var rewards = new RewardsRepositoryFalso();
        var transacoes = new RewardTransacaoRepositoryFalso();
        var service = new RewardsPontuacaoService(
            rewards,
            transacoes,
            new FakeTimeProvider(new DateTimeOffset(2026, 5, 24, 10, 0, 0, TimeSpan.Zero))
        );

        var atribuido = await service.AtribuirSeAindaNaoAtribuidoAsync(
            5,
            3,
            "Comentario publicado",
            "comentario:10"
        );

        Assert.True(atribuido);
        Assert.Equal(3, rewards.ObterSaldo(5));
        Assert.Single(transacoes.ObterHistorico(5));
    }

    [Fact]
    public async Task Rewards_nao_duplica_a_mesma_acao()
    {
        var rewards = new RewardsRepositoryFalso();
        var transacoes = new RewardTransacaoRepositoryFalso();
        var service = new RewardsPontuacaoService(
            rewards,
            transacoes,
            new FakeTimeProvider(DateTimeOffset.UtcNow)
        );

        await service.AtribuirSeAindaNaoAtribuidoAsync(5, 3, "Comentario publicado", "comentario:10");
        var segunda = await service.AtribuirSeAindaNaoAtribuidoAsync(5, 3, "Comentario publicado", "comentario:10");

        Assert.False(segunda);
        Assert.Equal(3, rewards.ObterSaldo(5));
        Assert.Single(transacoes.ObterHistorico(5));
    }

    [Fact]
    public void Reviews_mostram_10_por_defeito()
    {
        var reviews = Enumerable.Range(1, 12).Select(i => new OnlineCinemaFestival.Client.Models.AvaliacaoDTO
        {
            Id = i,
            Data = new DateTime(2026, 5, i),
        });

        var visiveis = ReviewListagemPolicy.ObterVisiveis(
            reviews,
            ReviewListagemPolicy.QuantidadeInicial
        );

        Assert.Equal(10, visiveis.Count);
        Assert.Equal(12, visiveis[0].Id);
    }

    [Fact]
    public void Ver_mais_reviews_carrega_mais_reviews()
    {
        var proximaQuantidade = ReviewListagemPolicy.ProximaQuantidade(25, 10);

        Assert.Equal(20, proximaQuantidade);
    }

    [Fact]
    public async Task Filtro_multi_genero_devolve_qualquer_genero_e_prioriza_maior_correspondencia()
    {
        var acao = new Genero { Name = "Acao" };
        var drama = new Genero { Name = "Drama" };
        var comedia = new Genero { Name = "Comedia" };
        var filmes = new List<Filme>
        {
            FilmeComGeneros(1, "So Acao", acao),
            FilmeComGeneros(2, "Acao e Drama", acao, drama),
            FilmeComGeneros(3, "So Comedia", comedia),
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
    public async Task Catalogo_ordena_por_popularidade_classificacao_titulo_e_lancamento()
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
    public async Task Tmdb_indisponivel_nao_causa_crash()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OnlineCinemaFestival.Api.Services.TmdbService(
            new TmdbApiClientIndisponivel(),
            cache
        );

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

    private static OnlineCinemaFestival.Api.Services.RecomendacaoService CriarRecomendacaoService(
        List<Filme> filmes,
        Utilizador? utilizador,
        params IRecomendacaoStrategy[] strategies
    )
    {
        return new OnlineCinemaFestival.Api.Services.RecomendacaoService(
            new FilmeRepositoryFalso(filmes),
            new UtilizadorRepositoryFalso(utilizador),
            strategies
        );
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

    private static Filme FilmeCatalogo(
        int id,
        string titulo,
        DateTime lancamento,
        double avaliacaoTmdb,
        int popularidade
    )
    {
        var filme = FilmeComAvaliacoes(id, titulo, Enumerable.Repeat(8, popularidade).ToArray());
        filme.DataLancamento = lancamento;
        filme.AvaliacaoTmdb = avaliacaoTmdb;
        return filme;
    }

    private static Filme FilmeComGeneros(int id, string titulo, params Genero[] generos)
    {
        var filme = new Filme { Id = id, Titulo = titulo, DataLancamento = new DateTime(2024, 1, 1) };
        foreach (var genero in generos)
        {
            filme.FilmeGeneros.Add(new FilmeGenero
            {
                Filme = filme,
                FilmeId = id,
                Genero = genero,
                GeneroId = genero.Id,
            });
        }

        return filme;
    }

    private static Filme FilmeComVisualizacoes(int id, string titulo, int visualizacoes)
    {
        var filme = new Filme { Id = id, Titulo = titulo, DataLancamento = new DateTime(2024, 1, 1) };
        for (var i = 0; i < visualizacoes; i++)
            filme.Visualizacoes.Add(new Visualizacao { Id = i + 1, FilmeId = id });

        return filme;
    }

    private static Filme FilmeComAvaliacoes(int id, string titulo, params int[] pontuacoes)
    {
        var filme = new Filme { Id = id, Titulo = titulo, DataLancamento = new DateTime(2024, 1, 1) };
        foreach (var pontuacao in pontuacoes)
            filme.Avaliacoes.Add(new Avaliacao { FilmeId = id, Pontuacao = pontuacao });

        return filme;
    }

    private sealed class FilmeRepositoryFalso : IFilmeRepository
    {
        private readonly List<Filme> _filmes;

        public FilmeRepositoryFalso(List<Filme> filmes)
        {
            _filmes = filmes;
        }

        public Task<IEnumerable<Filme>> ObterTodosAsync() => Task.FromResult<IEnumerable<Filme>>(_filmes);

        public Task<Filme?> ObterPorIdAsync(int id) =>
            Task.FromResult(_filmes.FirstOrDefault(f => f.Id == id));

        public Task<Filme?> ObterDetalhePorIdAsync(int id) => ObterPorIdAsync(id);

        public Task<Filme?> ObterPorTmdbIdAsync(int tmdbId) =>
            Task.FromResult(_filmes.FirstOrDefault(f => f.TmdbId == tmdbId));

        public Task<List<Filme>> ObterPrincipaisAsync(int quantidade) =>
            Task.FromResult(_filmes.Take(quantidade).ToList());

        public Task<List<Festival>> ObterFestivaisDoFilmeAsync(int filmeId) =>
            Task.FromResult(new List<Festival>());

        public Task<List<Sessao>> ObterSessoesDoFilmeAsync(int filmeId) =>
            Task.FromResult(new List<Sessao>());

        public Task<Genero> ObterOuCriarGeneroAsync(string nome) =>
            Task.FromResult(new Genero { Name = nome });

        public Task<Pessoa> ObterOuCriarPessoaAsync(int? tmdbPessoaId, string nome, string? imagemUrl) =>
            Task.FromResult(new Pessoa { TmdbPessoaId = tmdbPessoaId, Nome = nome, ImagemUrl = imagemUrl });

        public Task<bool> UtilizadorViuFilmeAsync(int utilizadorId, int filmeId) => Task.FromResult(false);

        public Task<Avaliacao?> ObterAvaliacaoAsync(int utilizadorId, int filmeId) => Task.FromResult<Avaliacao?>(null);

        public Task AddAvaliacaoAsync(Avaliacao avaliacao) => Task.CompletedTask;

        public Task AddAsync(Filme filme)
        {
            _filmes.Add(filme);
            return Task.CompletedTask;
        }

        public void AtualizarVideo(Filme filme, string? provider, string? key, string? url) { }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class UtilizadorRepositoryFalso : IUtilizadorRepository
    {
        private readonly Utilizador? _utilizador;

        public UtilizadorRepositoryFalso(Utilizador? utilizador)
        {
            _utilizador = utilizador;
        }

        public Task<Utilizador?> ObterPorIdAsync(int id) => Task.FromResult(_utilizador);
        public Task<Utilizador?> ObterPorEmailAsync(string email) => Task.FromResult(_utilizador);
        public Task<Utilizador?> ObterPorTelefoneAsync(string telefone) => Task.FromResult(_utilizador);
        public Task<Utilizador?> ObterComPerfilAsync(int id) => Task.FromResult(_utilizador);
        public Task<List<Utilizador>> ObterPerfisPublicosAsync() => Task.FromResult(new List<Utilizador>());
        public Task AddAsync(Utilizador utilizador) => Task.CompletedTask;
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

        public FestivalFilmeRepositoryFalso(List<Filme> filmes)
        {
            _filmes = filmes;
        }

        public Task<bool> ExisteAsync(int festivalId, int filmeId) => Task.FromResult(false);
        public Task<FestivalFilme?> ObterAsync(int festivalId, int filmeId) => Task.FromResult<FestivalFilme?>(null);
        public Task AdicionarAsync(FestivalFilme festivalFilme) => Task.CompletedTask;
        public void Remove(FestivalFilme festivalFilme) { }
        public Task<IEnumerable<Filme>> ObterFilmesPorFestivalIdAsync(int festivalId) => Task.FromResult<IEnumerable<Filme>>(_filmes);
        public Task<IEnumerable<FestivalFilme>> ObterAssociacoesPorFestivalIdAsync(int festivalId) => Task.FromResult(Enumerable.Empty<FestivalFilme>());
        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class RewardsRepositoryFalso : IRewardsRepository
    {
        private readonly Dictionary<int, int> _saldos = new();

        public Task AddOrUpdatePointsAsync(int utilizadorId, int pontosGanhos)
        {
            _saldos[utilizadorId] = ObterSaldo(utilizadorId) + pontosGanhos;
            return Task.CompletedTask;
        }

        public int ObterSaldo(int utilizadorId) =>
            _saldos.TryGetValue(utilizadorId, out var saldo) ? saldo : 0;
    }

    private sealed class RewardTransacaoRepositoryFalso : IRewardTransacaoRepository
    {
        private readonly List<RewardTransacao> _transacoes = new();

        public Task AddAsync(RewardTransacao transacao)
        {
            _transacoes.Add(transacao);
            return Task.CompletedTask;
        }

        public Task<bool> ExisteChaveAcaoAsync(int utilizadorId, string chaveAcao) =>
            Task.FromResult(_transacoes.Any(t => t.UtilizadorId == utilizadorId && t.ChaveAcao == chaveAcao));

        public List<RewardTransacao> ObterHistorico(int utilizadorId) =>
            _transacoes.Where(t => t.UtilizadorId == utilizadorId).ToList();

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class TmdbApiClientIndisponivel : ITmdbApiClient
    {
        public Task<T?> GetAsync<T>(string path) => throw new HttpRequestException("TMDB indisponivel");
    }
}
