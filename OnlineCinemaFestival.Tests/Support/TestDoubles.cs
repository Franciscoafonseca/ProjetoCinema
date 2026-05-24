using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Tests.Support;

public sealed class GeradorReferenciaCompraFalso : IGeradorReferenciaCompra
{
    private readonly string _referencia;

    public GeradorReferenciaCompraFalso(string referencia)
    {
        _referencia = referencia;
    }

    public string Gerar() => _referencia;
}

public sealed class CompraRepositoryFalso : ICompraRepository
{
    private int _sequencia;
    private readonly Dictionary<int, Compra> _compras = new();

    public Compra? UltimaCompraAdicionada { get; private set; }

    public Task AddAsync(Compra compra)
    {
        if (compra.Id == 0)
            compra.Id = ++_sequencia;

        _compras[compra.Id] = compra;
        UltimaCompraAdicionada = compra;
        return Task.CompletedTask;
    }

    public Task<Compra?> ObterPorIdAsync(int id)
    {
        _compras.TryGetValue(id, out var compra);
        return Task.FromResult(compra);
    }

    public Task<IEnumerable<Compra>> ObterPorUtilizadorIdAsync(int utilizadorId) =>
        Task.FromResult<IEnumerable<Compra>>(_compras.Values.Where(c => c.UtilizadorId == utilizadorId));

    public Task<List<Compra>> ObterHistoricoPorUtilizadorAsync(int utilizadorId) =>
        Task.FromResult(_compras.Values.Where(c => c.UtilizadorId == utilizadorId).ToList());

    public Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action) => action();

    public Task SaveChangesAsync() => Task.CompletedTask;
}

public sealed class CarrinhoCheckoutServiceFalso : ICarrinhoCheckoutService
{
    private readonly Carrinho _carrinho;
    private readonly TimeProvider _timeProvider;

    public bool CarrinhoLimpado { get; private set; }

    public CarrinhoCheckoutServiceFalso(Carrinho carrinho, TimeProvider timeProvider)
    {
        _carrinho = carrinho;
        _timeProvider = timeProvider;
    }

    public Task<Carrinho> ObterCarrinhoValidadoAsync(int utilizadorId) => Task.FromResult(_carrinho);

    public Task LimparCarrinhoAsync(Carrinho carrinho)
    {
        CarrinhoLimpado = true;
        carrinho.Itens.Clear();
        carrinho.AtualizadoEm = _timeProvider.GetUtcNow().UtcDateTime;
        return Task.CompletedTask;
    }
}

public sealed class AcessoUtilizadorFactoryFalso : IAcessoUtilizadorFactory
{
    public AcessoUtilizador Criar(int utilizadorId, Compra compra, CarrinhoItem item, DateTime dataCompra)
    {
        return new AcessoUtilizador
        {
            UtilizadorId = utilizadorId,
            CompraId = compra.Id,
            AcessoId = item.AcessoId,
            TipoAcesso = item.Acesso.Tipo,
            InicioValidade = dataCompra,
            FimValidade = dataCompra.AddHours(2),
            Ativo = true,
            CriadoEm = dataCompra,
        };
    }
}

public sealed class AcessoUtilizadorRepositoryFalso : IAcessoUtilizadorRepository
{
    public List<AcessoUtilizador> Adicionados { get; } = new();

    public Task AddRangeAsync(IEnumerable<AcessoUtilizador> acessos)
    {
        Adicionados.AddRange(acessos);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<AcessoUtilizador>> ObterPorUtilizadorIdAsync(int utilizadorId) =>
        Task.FromResult(Enumerable.Empty<AcessoUtilizador>());

    public Task<IEnumerable<AcessoUtilizador>> ObterAtivosPorUtilizadorIdAsync(
        int utilizadorId,
        DateTime dataAtual
    ) => Task.FromResult(Enumerable.Empty<AcessoUtilizador>());

    public Task<bool> ExisteAcessoAtivoAsync(int utilizadorId, int acessoId, DateTime dataAtual) =>
        Task.FromResult(false);

    public Task<AcessoUtilizador?> ObterAcessoValidoAsync(
        int utilizadorId,
        TipoAcesso tipoAcesso,
        DateTime dataAtual,
        int? filmeId = null,
        int? sessaoId = null,
        int? festivalId = null
    ) => Task.FromResult<AcessoUtilizador?>(null);

    public Task<AcessoUtilizador?> ObterPasseCompletoValidoParaFilmeAsync(
        int utilizadorId,
        int filmeId,
        int? festivalId,
        DateTime dataAtual
    ) => Task.FromResult<AcessoUtilizador?>(null);
}

public sealed class CompraObserverFalso : ICompraObserver
{
    public Task NotificarAsync(int utilizadorId, decimal valorTotal, List<Acesso> acessos) =>
        Task.CompletedTask;
}
