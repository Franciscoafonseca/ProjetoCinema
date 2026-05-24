using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support.Builders;

namespace OnlineCinemaFestival.Tests.Recomendacoes;

public class RecomendacaoTests
{
    [Fact]
    public async Task ObterRecomendacoes_PorGenero_UsaPreferenciasDoUtilizador()
    {
        var drama = new Genero { Id = 1, Name = "Drama" };
        var filmes = new List<Filme>
        {
            new FilmeBuilder().ComId(1).ComTitulo("Drama Forte").ComGenero(drama).Build(),
            new FilmeBuilder().ComId(2).ComTitulo("Comedia Leve")
                .ComGenero(new Genero { Id = 2, Name = "Comedia" }).Build(),
        };
        var utilizador = new UtilizadorBuilder().ComId(7).Build();
        utilizador.GenerosFavoritos.Add(new UtilizadorGeneroFavorito { Genero = drama });

        var service = CriarRecomendacaoService(filmes, utilizador, new RecomendacaoPorGeneroStrategy());

        var resultado = await service.ObterRecomendacoesAsync(utilizador.Id);

        Assert.Single(resultado);
        Assert.Equal("Drama Forte", resultado[0].Filme.Titulo);
        Assert.Contains("Genero", resultado[0].Motivo);
    }

    [Fact]
    public async Task ObterRecomendacoes_PorPopularidade_OrdenaPorVisualizacoes()
    {
        var filmes = new List<Filme>
        {
            new FilmeBuilder().ComId(1).ComTitulo("Pouco visto").ComVisualizacoes(1).Build(),
            new FilmeBuilder().ComId(2).ComTitulo("Muito visto").ComVisualizacoes(4).Build(),
        };

        var service = CriarRecomendacaoService(filmes, null, new RecomendacaoPorPopularidadeStrategy());

        var resultado = await service.ObterRecomendacoesAsync(1);

        Assert.Equal("Muito visto", resultado[0].Filme.Titulo);
        Assert.Contains("Popular", resultado[0].Motivo);
    }

    [Fact]
    public async Task ObterRecomendacoes_PorAvaliacao_OrdenaPorMedia()
    {
        var filmes = new List<Filme>
        {
            new FilmeBuilder().ComId(1).ComTitulo("Medio").ComAvaliacoes(6, 7).Build(),
            new FilmeBuilder().ComId(2).ComTitulo("Excelente").ComAvaliacoes(9, 10).Build(),
        };

        var service = CriarRecomendacaoService(filmes, null, new RecomendacaoPorAvaliacaoStrategy());

        var resultado = await service.ObterRecomendacoesAsync(1);

        Assert.Equal("Excelente", resultado[0].Filme.Titulo);
        Assert.Contains("avaliado", resultado[0].Motivo);
    }

    [Fact]
    public async Task ObterRecomendacoes_SemPreferencias_UsaFallbackDoCatalogo()
    {
        var filmes = new List<Filme>
        {
            new FilmeBuilder().ComId(1).ComTitulo("Catalogo A").ComVisualizacoes(0).Build(),
            new FilmeBuilder().ComId(2).ComTitulo("Catalogo B").ComVisualizacoes(2).Build(),
        };

        var service = CriarRecomendacaoService(filmes, null);

        var resultado = await service.ObterRecomendacoesAsync(1);

        Assert.Equal(2, resultado.Count);
        Assert.Equal("Catalogo B", resultado[0].Filme.Titulo);
        Assert.All(resultado, r => Assert.Contains("Sugestao geral", r.Motivo));
    }

    [Fact]
    public async Task ObterRecomendacoes_MesmoFilmeMultiplasStrategies_NaoDevolveDuplicados()
    {
        var filme = new FilmeBuilder().ComId(1).ComTitulo("Mesmo Filme").ComAvaliacoes(9).Build();
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

    private static RecomendacaoService CriarRecomendacaoService(
        List<Filme> filmes,
        Utilizador? utilizador,
        params IRecomendacaoStrategy[] strategies
    )
    {
        return new RecomendacaoService(
            new FilmeRepositoryFalso(filmes),
            new UtilizadorRepositoryUnicoFalso(utilizador),
            strategies
        );
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

    private sealed class UtilizadorRepositoryUnicoFalso : IUtilizadorRepository
    {
        private readonly Utilizador? _utilizador;
        public UtilizadorRepositoryUnicoFalso(Utilizador? utilizador) { _utilizador = utilizador; }

        public Task<Utilizador?> ObterPorIdAsync(int id) => Task.FromResult(_utilizador);
        public Task<Utilizador?> ObterPorEmailAsync(string email) => Task.FromResult(_utilizador);
        public Task<Utilizador?> ObterPorTelefoneAsync(string telefone) => Task.FromResult(_utilizador);
        public Task<Utilizador?> ObterComPerfilAsync(int id) => Task.FromResult(_utilizador);
        public Task<List<Utilizador>> ObterPerfisPublicosAsync() => Task.FromResult(new List<Utilizador>());
        public Task AddAsync(Utilizador utilizador) => Task.CompletedTask;
        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
