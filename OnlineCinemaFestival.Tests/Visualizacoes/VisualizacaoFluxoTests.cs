using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Services.PoliticasAcesso;
using OnlineCinemaFestival.Tests.Support;

namespace OnlineCinemaFestival.Tests.Visualizacoes;

public class VisualizacaoFluxoTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Filme_SemAcesso_DeveRejeitar()
    {
        var contexto = CriarContexto();

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            contexto.VisualizacaoService.ObterVisualizacaoFilmeAsync(7, 10, null)
        );

        Assert.Contains("Sem acesso", ex.Message);
        Assert.Empty(contexto.VisualizacaoRepository.Visualizacoes);
    }

    [Fact]
    public async Task Filme_ComAluguerExpirado_DeveRejeitarComoExpirado()
    {
        var acesso = CriarAcesso(
            TipoAcesso.AluguerDigital,
            filmeId: 10,
            inicio: Agora.UtcDateTime.AddHours(-3),
            fim: Agora.UtcDateTime.AddMinutes(-1)
        );
        var contexto = CriarContexto(acesso);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            contexto.VisualizacaoService.ObterVisualizacaoFilmeAsync(7, 10, null)
        );

        Assert.Contains("expirou", ex.Message);
        Assert.Empty(contexto.VisualizacaoRepository.Visualizacoes);
    }

    [Fact]
    public async Task Filme_ComAluguerValido_DevePermitirERegistarHistorico()
    {
        var acesso = CriarAcesso(
            TipoAcesso.AluguerDigital,
            filmeId: 10,
            inicio: Agora.UtcDateTime.AddHours(-1),
            fim: Agora.UtcDateTime.AddHours(1)
        );
        var contexto = CriarContexto(acesso);

        var resultado = await contexto.VisualizacaoService.ObterVisualizacaoFilmeAsync(7, 10, null);

        Assert.Equal("Filme", resultado.TipoConteudo);
        Assert.Single(resultado.Conteudos);
        Assert.Single(contexto.VisualizacaoRepository.Visualizacoes);
        Assert.Equal(Agora.UtcDateTime, contexto.VisualizacaoRepository.Visualizacoes[0].VisualizadoEm);
    }

    [Fact]
    public async Task Sessao_ComBilhete_AntesDoInicio_DeveRejeitarComoAindaNaoComecou()
    {
        var acesso = CriarAcesso(
            TipoAcesso.BilheteSessao,
            filmeId: 10,
            sessaoId: 5,
            festivalId: 2,
            inicio: Agora.UtcDateTime.AddHours(1),
            fim: Agora.UtcDateTime.AddHours(3)
        );
        var contexto = CriarContexto(acesso, sessaoInicio: Agora.UtcDateTime.AddHours(1));

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            contexto.VisualizacaoService.ObterVisualizacaoSessaoAsync(7, 5)
        );

        Assert.Contains("ainda nao comecou", ex.Message);
        Assert.Empty(contexto.VisualizacaoRepository.Visualizacoes);
    }

    [Fact]
    public async Task Sessao_ComBilhete_DepoisDoFim_DeveRejeitarComoTerminada()
    {
        var acesso = CriarAcesso(
            TipoAcesso.BilheteSessao,
            filmeId: 10,
            sessaoId: 5,
            festivalId: 2,
            inicio: Agora.UtcDateTime.AddHours(-3),
            fim: Agora.UtcDateTime.AddHours(-1)
        );
        var contexto = CriarContexto(
            acesso,
            sessaoInicio: Agora.UtcDateTime.AddHours(-3),
            sessaoFim: Agora.UtcDateTime.AddHours(-1)
        );

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            contexto.VisualizacaoService.ObterVisualizacaoSessaoAsync(7, 5)
        );

        Assert.Contains("terminou", ex.Message);
        Assert.Empty(contexto.VisualizacaoRepository.Visualizacoes);
    }

    [Fact]
    public async Task Sessao_ComBilhete_DentroDoHorario_DevePermitir()
    {
        var acesso = CriarAcesso(
            TipoAcesso.BilheteSessao,
            filmeId: 10,
            sessaoId: 5,
            festivalId: 2,
            inicio: Agora.UtcDateTime.AddMinutes(-30),
            fim: Agora.UtcDateTime.AddMinutes(30)
        );
        var contexto = CriarContexto(
            acesso,
            sessaoInicio: Agora.UtcDateTime.AddMinutes(-30),
            sessaoFim: Agora.UtcDateTime.AddMinutes(30)
        );

        var resultado = await contexto.VisualizacaoService.ObterVisualizacaoSessaoAsync(7, 5);

        Assert.Equal("Sessao", resultado.TipoConteudo);
        Assert.Single(contexto.VisualizacaoRepository.Visualizacoes);
    }

    private static ContextoVisualizacaoTeste CriarContexto(
        AcessoUtilizador? acesso = null,
        DateTime? sessaoInicio = null,
        DateTime? sessaoFim = null
    )
    {
        var filme = new Filme
        {
            Id = 10,
            Titulo = "Filme Teste",
            CapaUrl = "/poster.jpg",
            VideoUrl = "/videos/filme-teste.mp4",
        };
        var sessao = new Sessao
        {
            Id = 5,
            FilmeId = filme.Id,
            Filme = filme,
            FestivalId = 2,
            Festival = new Festival { Id = 2, Name = "Festival" },
            Inicio = sessaoInicio ?? Agora.UtcDateTime.AddMinutes(-30),
            Fim = sessaoFim ?? Agora.UtcDateTime.AddMinutes(30),
        };
        var visualizacaoRepository = new VisualizacaoRepositoryFalso(filme, sessao, new[] { 2 });
        var acessoRepository = new AcessoUtilizadorVisualizacaoRepositoryFalso(
            acesso is null ? [] : [acesso]
        );
        var timeProvider = new FakeTimeProvider(Agora);
        var acessoVisualizacaoService = new AcessoVisualizacaoService(
            acessoRepository,
            visualizacaoRepository,
            new PoliticaAcessoResolver(
                new IPoliticaAcesso[]
                {
                    new PoliticaBilheteSessao(),
                    new PoliticaPasseDiario(),
                    new PoliticaPasseCompleto(),
                    new PoliticaAluguerDigital(),
                }
            ),
            timeProvider
        );
        var visualizacaoService = new VisualizacaoService(
            visualizacaoRepository,
            acessoVisualizacaoService,
            Array.Empty<IVisualizacaoObserver>(),
            timeProvider
        );

        return new ContextoVisualizacaoTeste(visualizacaoService, visualizacaoRepository);
    }

    private static AcessoUtilizador CriarAcesso(
        TipoAcesso tipo,
        int? filmeId = null,
        int? sessaoId = null,
        int? festivalId = null,
        DateTime? inicio = null,
        DateTime? fim = null
    )
    {
        return new AcessoUtilizador
        {
            UtilizadorId = 7,
            AcessoId = 99,
            TipoAcesso = tipo,
            FilmeId = filmeId,
            SessaoId = sessaoId,
            FestivalId = festivalId,
            InicioValidade = inicio ?? Agora.UtcDateTime.AddMinutes(-30),
            FimValidade = fim ?? Agora.UtcDateTime.AddMinutes(30),
            Ativo = true,
        };
    }

    private sealed record ContextoVisualizacaoTeste(
        VisualizacaoService VisualizacaoService,
        VisualizacaoRepositoryFalso VisualizacaoRepository
    );

    private sealed class AcessoUtilizadorVisualizacaoRepositoryFalso : IAcessoUtilizadorRepository
    {
        private readonly List<AcessoUtilizador> _acessos;

        public AcessoUtilizadorVisualizacaoRepositoryFalso(IEnumerable<AcessoUtilizador> acessos)
        {
            _acessos = acessos.ToList();
        }

        public Task AddRangeAsync(IEnumerable<AcessoUtilizador> acessos)
        {
            _acessos.AddRange(acessos);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<AcessoUtilizador>> ObterPorUtilizadorIdAsync(int utilizadorId) =>
            Task.FromResult<IEnumerable<AcessoUtilizador>>(
                _acessos.Where(a => a.UtilizadorId == utilizadorId)
            );

        public Task<IEnumerable<AcessoUtilizador>> ObterAtivosPorUtilizadorIdAsync(
            int utilizadorId,
            DateTime dataAtual
        ) =>
            Task.FromResult<IEnumerable<AcessoUtilizador>>(
                _acessos.Where(a =>
                    a.UtilizadorId == utilizadorId
                    && a.Ativo
                    && a.InicioValidade <= dataAtual
                    && a.FimValidade >= dataAtual
                )
            );

        public Task<bool> ExisteAcessoAtivoAsync(
            int utilizadorId,
            int acessoId,
            DateTime dataAtual
        ) =>
            Task.FromResult(
                _acessos.Any(a =>
                    a.UtilizadorId == utilizadorId
                    && a.AcessoId == acessoId
                    && a.Ativo
                    && a.FimValidade > dataAtual
                )
            );

        public Task<bool> ExisteParaCompraAsync(int compraId) =>
            Task.FromResult(_acessos.Any(a => a.CompraId == compraId));
    }

    private sealed class VisualizacaoRepositoryFalso : IVisualizacaoRepository
    {
        private readonly Filme _filme;
        private readonly Sessao _sessao;
        private readonly IReadOnlySet<int> _festivalIdsDoFilme;

        public List<Visualizacao> Visualizacoes { get; } = new();

        public VisualizacaoRepositoryFalso(
            Filme filme,
            Sessao sessao,
            IEnumerable<int> festivalIdsDoFilme
        )
        {
            _filme = filme;
            _sessao = sessao;
            _festivalIdsDoFilme = festivalIdsDoFilme.ToHashSet();
        }

        public Task<Filme?> ObterFilmePorIdAsync(int filmeId) =>
            Task.FromResult(_filme.Id == filmeId ? _filme : null);

        public Task<Sessao?> ObterSessaoPorIdAsync(int sessaoId) =>
            Task.FromResult(_sessao.Id == sessaoId ? _sessao : null);

        public Task<bool> FilmePertenceAoFestivalAsync(int filmeId, int festivalId) =>
            Task.FromResult(_filme.Id == filmeId && _festivalIdsDoFilme.Contains(festivalId));

        public Task<IReadOnlySet<int>> ObterFestivalIdsDoFilmeAsync(int filmeId) =>
            Task.FromResult(
                _filme.Id == filmeId ? _festivalIdsDoFilme : new HashSet<int>()
            );

        public Task AddAsync(Visualizacao visualizacao)
        {
            Visualizacoes.Add(visualizacao);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<Visualizacao> visualizacoes)
        {
            Visualizacoes.AddRange(visualizacoes);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Visualizacao>> ObterPorUtilizadorIdAsync(int utilizadorId) =>
            Task.FromResult<IEnumerable<Visualizacao>>(
                Visualizacoes.Where(v => v.UtilizadorId == utilizadorId)
            );

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
