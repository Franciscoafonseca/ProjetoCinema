using Microsoft.Extensions.Configuration;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;

namespace OnlineCinemaFestival.Tests;

public class FluxosTemporaisTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 24, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Votacao_AntesDoInicio_ERejeitada()
    {
        var repo = new PremioFestivalRepositoryFalso(CriarPremio(Agora.AddHours(1), Agora.AddHours(2)));
        var service = new PremioFestivalService(repo, new FakeTimeProvider(Agora), Array.Empty<IVotoPremioObserver>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.VotarAsync(1, 10, 7));
    }

    [Fact]
    public async Task Votacao_DepoisDoFim_ERejeitada()
    {
        var repo = new PremioFestivalRepositoryFalso(CriarPremio(Agora.AddHours(-2), Agora.AddHours(-1)));
        var service = new PremioFestivalService(repo, new FakeTimeProvider(Agora), Array.Empty<IVotoPremioObserver>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.VotarAsync(1, 10, 7));
    }

    [Fact]
    public async Task UtilizadorSemVisualizacoesNecessarias_NaoVota()
    {
        var repo = new PremioFestivalRepositoryFalso(CriarPremio(Agora.AddHours(-1), Agora.AddHours(1)))
        {
            ViuTodosFilmesElegiveis = false,
        };
        var service = new PremioFestivalService(repo, new FakeTimeProvider(Agora), Array.Empty<IVotoPremioObserver>());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.VotarAsync(1, 10, 7));
    }

    [Fact]
    public async Task UtilizadorComVisualizacoesNecessarias_Vota()
    {
        var repo = new PremioFestivalRepositoryFalso(CriarPremio(Agora.AddHours(-1), Agora.AddHours(1)))
        {
            ViuTodosFilmesElegiveis = true,
        };
        var service = new PremioFestivalService(repo, new FakeTimeProvider(Agora), Array.Empty<IVotoPremioObserver>());

        await service.VotarAsync(1, 10, 7);

        Assert.Single(repo.Votos);
    }

    [Fact]
    public async Task VotoDuplicado_ERejeitado()
    {
        var repo = new PremioFestivalRepositoryFalso(CriarPremio(Agora.AddHours(-1), Agora.AddHours(1)))
        {
            ViuTodosFilmesElegiveis = true,
        };
        var service = new PremioFestivalService(repo, new FakeTimeProvider(Agora), Array.Empty<IVotoPremioObserver>());

        await service.VotarAsync(1, 10, 7);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.VotarAsync(1, 10, 7));
    }

    [Fact]
    public async Task Vencedor_EPublicadoAutomaticamente_AposFim()
    {
        var premio = CriarPremio(Agora.AddHours(-2), Agora.AddHours(-1));
        var repo = new PremioFestivalRepositoryFalso(premio)
        {
            Votos =
            {
                new VotoPremioFestival { PremioFestivalId = premio.Id, FilmeId = 10, UtilizadorId = 1 },
                new VotoPremioFestival { PremioFestivalId = premio.Id, FilmeId = 10, UtilizadorId = 2 },
                new VotoPremioFestival { PremioFestivalId = premio.Id, FilmeId = 11, UtilizadorId = 3 },
            },
        };
        var service = new PublicacaoPremiosService(repo, new FakeTimeProvider(Agora));

        var publicados = await service.PublicarResultadosPendentesAsync();

        Assert.Equal(1, publicados);
        Assert.Equal(EstadoPremio.Publicado, premio.EstadoPremio);
        Assert.Equal(10, premio.Resultado?.FilmeIdVencedor);
    }

    [Fact]
    public void FilmeVencedor_MostraPremio()
    {
        var premio = CriarPremio(Agora.AddHours(-2), Agora.AddHours(-1));
        premio.EstadoPremio = EstadoPremio.Publicado;
        var resultado = new ResultadoPremioFestival
        {
            PremioFestival = premio,
            PremioFestivalId = premio.Id,
            FilmeIdVencedor = 10,
            TotalVotos = 4,
            PublicadoEm = Agora.UtcDateTime,
        };
        var filme = new Filme
        {
            Id = 10,
            Titulo = "Filme vencedor",
            ResultadosPremiosFestival = { resultado },
        };
        resultado.FilmeVencedor = filme;

        var dto = FilmeMapper.MapToReadDTO(filme);

        var premioDto = Assert.Single(dto.ResultadosPremiosPublicados);
        Assert.Equal(premio.Nome, premioDto.NomePremio);
    }

    [Fact]
    public async Task Multibanco_GeraReferenciaPendente()
    {
        var pagamento = await CriarPagamentoService(new FakeTimeProvider(Agora))
            .ProcessarPagamentoSimuladoAsync(CriarCompra(), MetodosPagamento.ReferenciaMultibanco);

        Assert.Equal(EstadoPagamento.Pendente, pagamento.Estado);
        Assert.Equal("12345", pagamento.Entidade);
        Assert.False(string.IsNullOrWhiteSpace(pagamento.Referencia));
    }

    [Fact]
    public async Task ReferenciaMultibanco_ExpiraAposTresHoras()
    {
        var repo = new CompraRepositoryFalso();
        await repo.AddAsync(CriarCompraPendente(Agora.UtcDateTime));
        var service = CriarPagamentosPendentesService(repo, Agora.AddHours(3).AddSeconds(1));

        var expirados = await service.ExpirarPendentesAsync();
        var compra = (await repo.ObterPorIdAsync(1))!;

        Assert.Equal(1, expirados);
        Assert.Equal(EstadoPagamento.Expirado, compra.Pagamento!.Estado);
        Assert.Equal(EstadoCompra.Cancelado, compra.Estado);
    }

    [Fact]
    public async Task PagamentoExpirado_NaoPodeSerConcluido()
    {
        var repo = new CompraRepositoryFalso();
        await repo.AddAsync(CriarCompraPendente(Agora.UtcDateTime));
        var service = CriarPagamentosPendentesService(repo, Agora.AddHours(4));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ConfirmarPagamentoAsync(1, 7)
        );
    }

    [Fact]
    public async Task PagamentosPendentes_AparecemNaAreaCorreta()
    {
        var repo = new CompraRepositoryFalso();
        await repo.AddAsync(CriarCompraPendente(Agora.UtcDateTime));
        var service = CriarPagamentosPendentesService(repo, Agora.AddHours(1));

        var pagamentos = await service.ObterDoUtilizadorAsync(7);

        var pagamento = Assert.Single(pagamentos);
        Assert.Equal(EstadoPagamento.Pendente, pagamento.Pagamento!.Estado);
    }

    [Fact]
    public async Task Chat_ApareceDuranteSessao()
    {
        var service = CriarChatService(CriarSessao(Agora.AddMinutes(-5), Agora.AddMinutes(5)), Agora);

        var entrada = await service.EntrarNaSessaoAsync(20, 7, administrador: false);

        Assert.Equal(20, entrada.SessaoId);
    }

    [Fact]
    public async Task Chat_DesapareceAposFimDaSessao()
    {
        var service = CriarChatService(CriarSessao(Agora.AddMinutes(-20), Agora.AddMinutes(-1)), Agora);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EntrarNaSessaoAsync(20, 7, administrador: false)
        );
    }

    [Fact]
    public async Task Backend_RejeitaMensagemAposFimDaSessao()
    {
        var service = CriarChatService(CriarSessao(Agora.AddMinutes(-20), Agora.AddMinutes(-1)), Agora);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EnviarMensagemAsync(20, 7, administrador: false, "Ola comunidade")
        );
    }

    private static PremioFestival CriarPremio(DateTimeOffset inicio, DateTimeOffset fim)
    {
        return new PremioFestival
        {
            Id = 1,
            FestivalId = 99,
            Nome = "Premio do publico",
            DataAberturaVotacao = inicio.UtcDateTime,
            DataFechoVotacao = fim.UtcDateTime,
            EstadoPremio = EstadoPremio.Aberto,
            Festival = new Festival { Id = 99, Name = "Festival" },
        };
    }

    private static Compra CriarCompra()
    {
        return new Compra
        {
            Id = 1,
            Referencia = "CMP-1",
            UtilizadorId = 7,
            ValorTotal = 15m,
        };
    }

    private static Compra CriarCompraPendente(DateTime criadoEm)
    {
        var compra = CriarCompra();
        compra.Estado = EstadoCompra.Pendente;
        compra.Pagamento = new Pagamento
        {
            Compra = compra,
            Referencia = "123456789",
            Entidade = "12345",
            Metodo = MetodosPagamento.ReferenciaMultibanco,
            Estado = EstadoPagamento.Pendente,
            CriadoEm = criadoEm,
            Valor = compra.ValorTotal,
        };

        return compra;
    }

    private static IPagamentoService CriarPagamentoService(TimeProvider timeProvider)
    {
        return new PagamentoSimuladoService(
            new IPagamentoStrategy[]
            {
                new PagamentoAprovadoSimuladoStrategy(),
                new PagamentoReferenciaMultibancoStrategy(CriarConfiguracaoPagamentos()),
            },
            timeProvider
        );
    }

    private static PagamentosPendentesService CriarPagamentosPendentesService(
        ICompraRepository repo,
        DateTimeOffset agora
    )
    {
        return new PagamentosPendentesService(
            repo,
            CriarConfiguracaoPagamentos(),
            new FakeTimeProvider(agora)
        );
    }

    private static IConfiguration CriarConfiguracaoPagamentos()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Pagamentos:Multibanco:Entidade"] = "12345",
                    ["Pagamentos:Multibanco:ExpiracaoHoras"] = "3",
                }
            )
            .Build();
    }

    private static ChatSessaoService CriarChatService(Sessao sessao, DateTimeOffset agora)
    {
        return new ChatSessaoService(
            new MensagemChatSessaoRepositoryFalso(sessao),
            new AcessoVisualizacaoServiceFalso(),
            new UtilizadorRepositoryFalso(new Utilizador { Id = 7, Name = "Utilizador" }),
            new FakeTimeProvider(agora)
        );
    }

    private static Sessao CriarSessao(DateTimeOffset inicio, DateTimeOffset fim)
    {
        return new Sessao
        {
            Id = 20,
            Inicio = inicio.UtcDateTime,
            Fim = fim.UtcDateTime,
            TemChatAoVivo = true,
            Filme = new Filme { Id = 10, Titulo = "Filme" },
            Festival = new Festival { Id = 99, Name = "Festival" },
        };
    }

    private sealed class PremioFestivalRepositoryFalso : IPremioFestivalRepository
    {
        private readonly PremioFestival _premio;

        public PremioFestivalRepositoryFalso(PremioFestival premio)
        {
            _premio = premio;
        }

        public bool ViuTodosFilmesElegiveis { get; init; } = true;

        public List<VotoPremioFestival> Votos { get; init; } = new();

        public Task<bool> FestivalExisteAsync(int festivalId) => Task.FromResult(festivalId == _premio.FestivalId);

        public Task<PremioFestival?> ObterPremioAsync(int premioFestivalId) =>
            Task.FromResult(premioFestivalId == _premio.Id ? _premio : null);

        public Task<PremioFestival?> ObterPremioComResultadoAsync(int premioFestivalId) =>
            Task.FromResult(premioFestivalId == _premio.Id ? _premio : null);

        public Task AddPremioAsync(PremioFestival premio) => Task.CompletedTask;

        public Task<bool> FilmeElegivelAsync(int festivalId, int filmeId) => Task.FromResult(filmeId is 10 or 11);

        public Task<bool> UtilizadorJaVotouAsync(int premioFestivalId, int utilizadorId) =>
            Task.FromResult(Votos.Any(v => v.PremioFestivalId == premioFestivalId && v.UtilizadorId == utilizadorId));

        public Task<bool> UtilizadorViuTodosFilmesElegiveisAsync(int festivalId, int utilizadorId) =>
            Task.FromResult(ViuTodosFilmesElegiveis);

        public Task AddVotoAsync(VotoPremioFestival voto)
        {
            Votos.Add(voto);
            return Task.CompletedTask;
        }

        public Task<(int FilmeId, int TotalVotos)?> ObterVencedorPorVotosAsync(int premioFestivalId)
        {
            var vencedor = Votos.Where(v => v.PremioFestivalId == premioFestivalId)
                .GroupBy(v => v.FilmeId)
                .Select(g => (FilmeId: g.Key, TotalVotos: g.Count()))
                .OrderByDescending(v => v.TotalVotos)
                .ThenBy(v => v.FilmeId)
                .FirstOrDefault();

            return Task.FromResult<(int FilmeId, int TotalVotos)?>(vencedor == default ? null : vencedor);
        }

        public Task AddResultadoAsync(ResultadoPremioFestival resultado)
        {
            _premio.Resultado = resultado;
            return Task.CompletedTask;
        }

        public Task<ResultadoPremioFestival?> ObterResultadoCompletoAsync(int premioFestivalId) =>
            Task.FromResult(_premio.Resultado);

        public Task<List<ResultadoPremioFestival>> ObterResultadosPublicosAsync(int? festivalId, int? filmeId) =>
            Task.FromResult(_premio.Resultado is null ? new List<ResultadoPremioFestival>() : new List<ResultadoPremioFestival> { _premio.Resultado });

        public Task<List<PremioFestival>> ObterPremiosPorFestivalAsync(int festivalId, bool incluirRascunhos) =>
            Task.FromResult(new List<PremioFestival> { _premio });

        public Task<List<PremioFestival>> ObterPremiosPendentesPublicacaoAsync(DateTime dataAtual) =>
            Task.FromResult(_premio.Resultado == null && _premio.DataFechoVotacao <= dataAtual ? new List<PremioFestival> { _premio } : new List<PremioFestival>());

        public Task SaveChangesAsync() => Task.CompletedTask;

        public Task<bool> TrySaveChangesAsync() => Task.FromResult(true);
    }

    private sealed class MensagemChatSessaoRepositoryFalso : IMensagemChatSessaoRepository
    {
        private readonly Sessao _sessao;

        public MensagemChatSessaoRepositoryFalso(Sessao sessao)
        {
            _sessao = sessao;
        }

        public Task<Sessao?> ObterSessaoPorIdAsync(int sessaoId) =>
            Task.FromResult(sessaoId == _sessao.Id ? _sessao : null);

        public Task AdicionarAsync(MensagemChatSessao mensagem) => Task.CompletedTask;

        public Task<IReadOnlyList<MensagemChatSessao>> ListarHistoricoRecenteAsync(int sessaoId, int quantidade) =>
            Task.FromResult<IReadOnlyList<MensagemChatSessao>>(Array.Empty<MensagemChatSessao>());

        public Task<IReadOnlyList<MensagemChatSessao>> ListarMensagensRecentesDoUtilizadorAsync(int sessaoId, int utilizadorId, DateTime desde) =>
            Task.FromResult<IReadOnlyList<MensagemChatSessao>>(Array.Empty<MensagemChatSessao>());

        public Task<MensagemChatSessao?> ObterMensagemPorIdAsync(string mensagemId) => Task.FromResult<MensagemChatSessao?>(null);

        public void MarcarMensagemRemovida(MensagemChatSessao mensagem) { }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class AcessoVisualizacaoServiceFalso : IAcessoVisualizacaoService
    {
        public Task<AcessoUtilizador?> ObterAcessoValidoParaFilmeAsync(int utilizadorId, Filme filme, int? festivalId) =>
            Task.FromResult<AcessoUtilizador?>(new AcessoUtilizador { UtilizadorId = utilizadorId });

        public Task<AcessoUtilizador?> ObterAcessoValidoParaSessaoAsync(int utilizadorId, Sessao sessao) =>
            Task.FromResult<AcessoUtilizador?>(new AcessoUtilizador { UtilizadorId = utilizadorId });

        public Task<ResultadoAcessoVisualizacao> ObterResultadoParaFilmeAsync(int utilizadorId, Filme filme, int? festivalId) =>
            Task.FromResult(ResultadoAcessoVisualizacao.Autorizado(new AcessoUtilizador { UtilizadorId = utilizadorId }));

        public Task<ResultadoAcessoVisualizacao> ObterResultadoParaSessaoAsync(int utilizadorId, Sessao sessao) =>
            Task.FromResult(ResultadoAcessoVisualizacao.Autorizado(new AcessoUtilizador { UtilizadorId = utilizadorId }));

        public Task<bool> PodeVisualizarFilmeAsync(int utilizadorId, int filmeId, int? festivalId) => Task.FromResult(true);

        public Task<bool> PodeVisualizarSessaoAsync(int utilizadorId, Sessao sessao) => Task.FromResult(true);
    }

    private sealed class UtilizadorRepositoryFalso : IUtilizadorRepository
    {
        private readonly Utilizador _utilizador;

        public UtilizadorRepositoryFalso(Utilizador utilizador)
        {
            _utilizador = utilizador;
        }

        public Task<Utilizador?> ObterPorIdAsync(int id) => Task.FromResult(id == _utilizador.Id ? _utilizador : null);
        public Task<Utilizador?> ObterPorEmailAsync(string email) => Task.FromResult<Utilizador?>(null);
        public Task<Utilizador?> ObterPorTelefoneAsync(string telefone) => Task.FromResult<Utilizador?>(null);
        public Task<Utilizador?> ObterComPerfilAsync(int id) => ObterPorIdAsync(id);
        public Task<List<Utilizador>> ObterPerfisPublicosAsync() => Task.FromResult(new List<Utilizador>());
        public Task AddAsync(Utilizador utilizador) => Task.CompletedTask;
        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
