using Microsoft.Extensions.Configuration;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;

namespace OnlineCinemaFestival.Tests;

public class CarrinhoCheckoutAcessosTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Carrinho_DeveAdicionarBilheteSessaoComQuantidadeECalcularTotal()
    {
        var contexto = CriarContextoCarrinho();

        var resultado = await contexto.Service.AdicionarItemAsync(
            7,
            new CarrinhoItemCreateDTO
            {
                TipoAcesso = TipoAcesso.BilheteSessao,
                SessaoId = 10,
                Quantidade = 3,
            }
        );

        Assert.Single(resultado.Itens);
        Assert.Equal(3, resultado.Itens[0].Quantidade);
        Assert.Equal(30m, resultado.Itens[0].Subtotal);
        Assert.Equal(30m, resultado.Total);
    }

    [Fact]
    public async Task ValidatorPasseDiario_DeveRejeitarDataForaDoFestival()
    {
        var contexto = CriarContextoCarrinho();
        var validator = contexto.Validators.OfType<PasseDiarioCarrinhoItemValidator>().Single();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            validator.ValidarAlvoAsync(
                new CarrinhoItemCreateDTO
                {
                    TipoAcesso = TipoAcesso.PasseDiario,
                    FestivalId = 20,
                    DataAcesso = new DateTime(2026, 6, 1),
                    Quantidade = 1,
                }
            )
        );

        Assert.Contains("dentro do periodo do festival", ex.Message);
    }

    [Fact]
    public async Task ValidatorAluguerDigital_DeveRejeitarFilmeInexistente()
    {
        var contexto = CriarContextoCarrinho();
        var validator = contexto.Validators.OfType<AluguerDigitalCarrinhoItemValidator>().Single();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            validator.ValidarAlvoAsync(
                new CarrinhoItemCreateDTO
                {
                    TipoAcesso = TipoAcesso.AluguerDigital,
                    FilmeId = 999,
                    Quantidade = 1,
                }
            )
        );
    }

    [Fact]
    public async Task Checkout_DeveRejeitarAcessoInativo()
    {
        var contexto = CriarContextoCarrinho();
        contexto.AcessoBilhete.IsAtivo = false;
        var carrinho = new Carrinho
        {
            UtilizadorId = 7,
            Itens = new List<CarrinhoItem>
            {
                new()
                {
                    AcessoId = contexto.AcessoBilhete.Id,
                    Acesso = contexto.AcessoBilhete,
                    PrecoUnitario = 10m,
                    Quantidade = 1,
                },
            },
        };
        var validador = new ValidadorFinalizacaoCompra(
            new AcessoUtilizadorRepositoryFalso(),
            contexto.Validators,
            new FakeTimeProvider(Agora),
            CriarConfiguracao()
        );

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            validador.ValidarAsync(7, carrinho)
        );
    }

    [Fact]
    public async Task Checkout_DeveRejeitarCarrinhoVazio()
    {
        var contexto = CriarContextoCarrinho();
        var validador = new ValidadorFinalizacaoCompra(
            new AcessoUtilizadorRepositoryFalso(),
            contexto.Validators,
            new FakeTimeProvider(Agora),
            CriarConfiguracao()
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            validador.ValidarAsync(7, new Carrinho { UtilizadorId = 7 })
        );

        Assert.Contains("vazio", ex.Message);
    }

    [Fact]
    public async Task AcessoCompra_DeveCriarUmAcessoPorQuantidadeEManterIdempotencia()
    {
        var acessoRepo = new AcessoUtilizadorRepositoryFalso();
        var timeProvider = new FakeTimeProvider(Agora);
        var service = new AcessoCompraService(
            new AcessoUtilizadorFactoryFalso(),
            acessoRepo,
            timeProvider
        );
        var acesso = new Acesso
        {
            Id = 33,
            Nome = "Bilhete Sessao",
            Tipo = TipoAcesso.BilheteSessao,
            IsAtivo = true,
            Preco = 10m,
        };
        var compra = new Compra
        {
            Id = 5,
            UtilizadorId = 7,
            Pagamento = new Pagamento { Estado = EstadoPagamento.Aprovado },
        };
        var carrinho = new Carrinho
        {
            Itens = new List<CarrinhoItem>
            {
                new()
                {
                    AcessoId = acesso.Id,
                    Acesso = acesso,
                    PrecoUnitario = 10m,
                    Quantidade = 2,
                },
            },
        };

        var primeiraExecucao = await service.CriarAcessosSeAprovadoAsync(7, compra, carrinho);
        var segundaExecucao = await service.CriarAcessosSeAprovadoAsync(7, compra, carrinho);

        Assert.Equal(2, primeiraExecucao);
        Assert.Equal(0, segundaExecucao);
        Assert.Equal(2, acessoRepo.Adicionados.Count);
    }

    [Fact]
    public void FabricaAcessoUtilizador_DeveCriarJanelaTemporalParaAluguerDigital()
    {
        var configuracao = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { ["Acessos:DuracaoAluguerDigitalHoras"] = "48" }
            )
            .Build();
        var fabrica = new FabricaAcessoUtilizador(
            new IEstrategiaCriacaoAcessoUtilizador[]
            {
                new EstrategiaCriacaoAluguerDigital(configuracao),
            }
        );
        var compra = new Compra { Id = 5, UtilizadorId = 7 };
        var acesso = new Acesso
        {
            Id = 44,
            Tipo = TipoAcesso.AluguerDigital,
            FilmeId = 3,
            DuracaoHoras = 24,
        };
        var item = new CarrinhoItem { AcessoId = acesso.Id, Acesso = acesso };
        var dataCompra = Agora.UtcDateTime;

        var acessoUtilizador = fabrica.Criar(7, compra, item, dataCompra);

        Assert.Equal(dataCompra, acessoUtilizador.InicioValidade);
        Assert.Equal(dataCompra.AddHours(24), acessoUtilizador.FimValidade);
        Assert.Equal(3, acessoUtilizador.FilmeId);
    }

    private static ContextoCarrinho CriarContextoCarrinho()
    {
        var timeProvider = new FakeTimeProvider(Agora);
        var festival = new Festival
        {
            Id = 20,
            Name = "Festival",
            StartDate = new DateTime(2026, 5, 23),
            EndDate = new DateTime(2026, 5, 25),
        };
        var filme = new Filme { Id = 30, Titulo = "Filme" };
        var sessao = new Sessao
        {
            Id = 10,
            FestivalId = festival.Id,
            Festival = festival,
            FilmeId = filme.Id,
            Filme = filme,
            Inicio = new DateTime(2026, 5, 24, 20, 0, 0),
            Fim = new DateTime(2026, 5, 24, 22, 0, 0),
        };
        var acessoBilhete = new Acesso
        {
            Id = 100,
            Nome = "Bilhete Sessao",
            Tipo = TipoAcesso.BilheteSessao,
            IsAtivo = true,
            Preco = 10m,
            SessaoId = sessao.Id,
            Sessao = sessao,
        };
        var acessoPasseDiario = new Acesso
        {
            Id = 101,
            Nome = "Passe Diario",
            Tipo = TipoAcesso.PasseDiario,
            IsAtivo = true,
            Preco = 15m,
            FestivalId = festival.Id,
            Festival = festival,
            DataAcesso = new DateTime(2026, 5, 24),
        };
        var acessoPasseCompleto = new Acesso
        {
            Id = 102,
            Nome = "Passe Completo",
            Tipo = TipoAcesso.PasseCompleto,
            IsAtivo = true,
            Preco = 40m,
            FestivalId = festival.Id,
            Festival = festival,
        };
        var acessoAluguer = new Acesso
        {
            Id = 103,
            Nome = "Aluguer",
            Tipo = TipoAcesso.AluguerDigital,
            IsAtivo = true,
            Preco = 5m,
            FilmeId = filme.Id,
            Filme = filme,
            DuracaoHoras = 48,
        };
        var acessoRepository = new AcessoRepositoryFalso(
            acessoBilhete,
            acessoPasseDiario,
            acessoPasseCompleto,
            acessoAluguer
        );
        var validators = new ICarrinhoItemValidator[]
        {
            new BilheteSessaoCarrinhoItemValidator(
                new SessaoRepositoryFalso(sessao),
                acessoRepository,
                timeProvider
            ),
            new PasseDiarioCarrinhoItemValidator(
                new FestivalRepositoryFalso(festival),
                acessoRepository,
                timeProvider
            ),
            new PasseCompletoCarrinhoItemValidator(
                new FestivalRepositoryFalso(festival),
                acessoRepository,
                timeProvider
            ),
            new AluguerDigitalCarrinhoItemValidator(
                new FilmeRepositoryFalso(filme),
                acessoRepository
            ),
        };
        var carrinhoRepository = new CarrinhoRepositoryFalso(acessoRepository.ObterPorIdLocal);
        var service = new CarrinhoService(
            carrinhoRepository,
            acessoRepository,
            new AcessoUtilizadorRepositoryFalso(),
            validators,
            timeProvider,
            CriarConfiguracao()
        );

        return new ContextoCarrinho(service, validators, acessoBilhete);
    }

    private static IConfiguration CriarConfiguracao()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { ["Acessos:QuantidadeMaximaCarrinho"] = "5" }
            )
            .Build();
    }

    private sealed record ContextoCarrinho(
        CarrinhoService Service,
        IReadOnlyCollection<ICarrinhoItemValidator> Validators,
        Acesso AcessoBilhete
    );

    private sealed class AcessoRepositoryFalso : IAcessoRepository
    {
        private readonly List<Acesso> _acessos;

        public AcessoRepositoryFalso(params Acesso[] acessos)
        {
            _acessos = acessos.ToList();
        }

        public Acesso? ObterPorIdLocal(int id) => _acessos.FirstOrDefault(a => a.Id == id);

        public Task<IEnumerable<Acesso>> ObterTodosAsync() =>
            Task.FromResult<IEnumerable<Acesso>>(_acessos);

        public Task<Acesso?> ObterPorIdAsync(int id) => Task.FromResult(ObterPorIdLocal(id));

        public Task<Acesso?> ObterBilheteSessaoAtivoAsync(int sessaoId) =>
            Task.FromResult(
                _acessos.FirstOrDefault(a =>
                    a.IsAtivo && a.Tipo == TipoAcesso.BilheteSessao && a.SessaoId == sessaoId
                )
            );

        public Task<Acesso?> ObterPasseDiarioAtivoAsync(int festivalId, DateTime dataAcesso) =>
            Task.FromResult(
                _acessos.FirstOrDefault(a =>
                    a.IsAtivo
                    && a.Tipo == TipoAcesso.PasseDiario
                    && a.FestivalId == festivalId
                    && a.DataAcesso?.Date == dataAcesso.Date
                )
            );

        public Task<Acesso?> ObterPasseCompletoAtivoAsync(int festivalId) =>
            Task.FromResult(
                _acessos.FirstOrDefault(a =>
                    a.IsAtivo && a.Tipo == TipoAcesso.PasseCompleto && a.FestivalId == festivalId
                )
            );

        public Task<Acesso?> ObterAluguerDigitalAtivoAsync(int filmeId) =>
            Task.FromResult(
                _acessos.FirstOrDefault(a =>
                    a.IsAtivo && a.Tipo == TipoAcesso.AluguerDigital && a.FilmeId == filmeId
                )
            );

        public Task AddAsync(Acesso acesso)
        {
            _acessos.Add(acesso);
            return Task.CompletedTask;
        }

        public Task AddManyAsync(IEnumerable<Acesso> acessos)
        {
            _acessos.AddRange(acessos);
            return Task.CompletedTask;
        }

        public void Remove(Acesso acesso) => _acessos.Remove(acesso);

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class CarrinhoRepositoryFalso : ICarrinhoRepository
    {
        private readonly Func<int, Acesso?> _resolverAcesso;
        private readonly Carrinho _carrinho = new() { Id = 1, UtilizadorId = 7 };
        private int _sequenciaItens;

        public CarrinhoRepositoryFalso(Func<int, Acesso?> resolverAcesso)
        {
            _resolverAcesso = resolverAcesso;
        }

        public Task<Carrinho?> ObterPorUtilizadorIdAsync(int utilizadorId) =>
            Task.FromResult<Carrinho?>(_carrinho.UtilizadorId == utilizadorId ? _carrinho : null);

        public Task<Carrinho> ObterOuCriarPorUtilizadorIdAsync(int utilizadorId) =>
            Task.FromResult(_carrinho);

        public Task<CarrinhoItem?> ObterItemAsync(int carrinhoId, int itemId) =>
            Task.FromResult(_carrinho.Itens.FirstOrDefault(i => i.Id == itemId));

        public Task<CarrinhoItem?> ObterItemPorAcessoAsync(int carrinhoId, int acessoId) =>
            Task.FromResult(_carrinho.Itens.FirstOrDefault(i => i.AcessoId == acessoId));

        public Task<bool> ExisteItemComAcessoAsync(int carrinhoId, int acessoId) =>
            Task.FromResult(_carrinho.Itens.Any(i => i.AcessoId == acessoId));

        public Task AddItemAsync(CarrinhoItem item)
        {
            item.Id = ++_sequenciaItens;
            item.Acesso = _resolverAcesso(item.AcessoId)!;
            _carrinho.Itens.Add(item);
            return Task.CompletedTask;
        }

        public void RemoveItem(CarrinhoItem item) => _carrinho.Itens.Remove(item);

        public void RemoveItems(IEnumerable<CarrinhoItem> itens)
        {
            foreach (var item in itens.ToList())
                _carrinho.Itens.Remove(item);
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class SessaoRepositoryFalso : ISessaoRepository
    {
        private readonly Sessao? _sessao;

        public SessaoRepositoryFalso(Sessao? sessao)
        {
            _sessao = sessao;
        }

        public Task<IEnumerable<Sessao>> ObterTodosAsync() =>
            Task.FromResult<IEnumerable<Sessao>>(_sessao is null ? [] : [_sessao]);

        public Task<Sessao?> ObterPorIdAsync(int id) =>
            Task.FromResult(_sessao?.Id == id ? _sessao : null);

        public Task<IEnumerable<Sessao>> ObterPorFestivalIdAsync(int festivalId) =>
            Task.FromResult<IEnumerable<Sessao>>([]);

        public Task<IEnumerable<Sessao>> ObterPorFilmeIdAsync(int filmeId) =>
            Task.FromResult<IEnumerable<Sessao>>([]);

        public Task<IEnumerable<Sessao>> ObterDisponiveisAsync(DateTime dataAtual) =>
            Task.FromResult<IEnumerable<Sessao>>([]);

        public Task<bool> HasOverlapAsync(
            int festivalId,
            int filmeId,
            DateTime inicio,
            DateTime fim,
            int? ignoreSessaoId = null
        ) => Task.FromResult(false);

        public Task<bool> HasAcessosAssociadosAsync(int sessaoId) => Task.FromResult(false);

        public Task AddAsync(Sessao sessao) => Task.CompletedTask;

        public void Remove(Sessao sessao) { }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class FestivalRepositoryFalso : IFestivalRepository
    {
        private readonly Festival? _festival;

        public FestivalRepositoryFalso(Festival? festival)
        {
            _festival = festival;
        }

        public Task<IEnumerable<Festival>> ObterTodosAsync() =>
            Task.FromResult<IEnumerable<Festival>>(_festival is null ? [] : [_festival]);

        public Task<Festival?> ObterPorIdAsync(int id) =>
            Task.FromResult(_festival?.Id == id ? _festival : null);

        public Task<Festival?> ObterDetalhePorIdAsync(int id) => ObterPorIdAsync(id);

        public Task AddAsync(Festival festival) => Task.CompletedTask;

        public void Remove(Festival festival) { }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class FilmeRepositoryFalso : IFilmeRepository
    {
        private readonly Filme? _filme;

        public FilmeRepositoryFalso(Filme? filme)
        {
            _filme = filme;
        }

        public Task<IEnumerable<Filme>> ObterTodosAsync() =>
            Task.FromResult<IEnumerable<Filme>>(_filme is null ? [] : [_filme]);

        public Task<Filme?> ObterPorIdAsync(int id) =>
            Task.FromResult(_filme?.Id == id ? _filme : null);

        public Task<Filme?> ObterDetalhePorIdAsync(int id) => ObterPorIdAsync(id);

        public Task<Filme?> ObterPorTmdbIdAsync(int tmdbId) => Task.FromResult<Filme?>(null);

        public Task<List<Filme>> ObterPrincipaisAsync(int quantidade) =>
            Task.FromResult(new List<Filme>());

        public Task<List<Festival>> ObterFestivaisDoFilmeAsync(int filmeId) =>
            Task.FromResult(new List<Festival>());

        public Task<List<Sessao>> ObterSessoesDoFilmeAsync(int filmeId) =>
            Task.FromResult(new List<Sessao>());

        public Task<Genero> ObterOuCriarGeneroAsync(string nome) =>
            Task.FromResult(new Genero { Name = nome });

        public Task<Pessoa> ObterOuCriarPessoaAsync(
            int? tmdbPessoaId,
            string nome,
            string? imagemUrl
        ) => Task.FromResult(new Pessoa { Nome = nome, ImagemUrl = imagemUrl });

        public Task<bool> UtilizadorViuFilmeAsync(int utilizadorId, int filmeId) =>
            Task.FromResult(false);

        public Task<Avaliacao?> ObterAvaliacaoAsync(int utilizadorId, int filmeId) =>
            Task.FromResult<Avaliacao?>(null);

        public Task AddAvaliacaoAsync(Avaliacao avaliacao) => Task.CompletedTask;

        public Task AddAsync(Filme filme) => Task.CompletedTask;

        public void AtualizarVideo(Filme filme, string? provider, string? key, string? url) { }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
