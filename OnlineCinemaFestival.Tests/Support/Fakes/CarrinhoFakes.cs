using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Tests.Support.Fakes;

public sealed class AcessoRepositoryFalso : IAcessoRepository
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

public sealed class CarrinhoRepositoryFalso : ICarrinhoRepository
{
    private readonly Func<int, Acesso?> _resolverAcesso;
    private readonly Carrinho _carrinho;
    private int _sequenciaItens;

    public CarrinhoRepositoryFalso(int utilizadorId, Func<int, Acesso?> resolverAcesso)
    {
        _resolverAcesso = resolverAcesso;
        _carrinho = new Carrinho { Id = 1, UtilizadorId = utilizadorId };
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

public sealed class SessaoRepositoryUnicoFalso : ISessaoRepository
{
    private readonly Sessao? _sessao;

    public SessaoRepositoryUnicoFalso(Sessao? sessao)
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

public sealed class FestivalRepositoryUnicoFalso : IFestivalRepository
{
    private readonly Festival? _festival;

    public FestivalRepositoryUnicoFalso(Festival? festival)
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

public sealed class FilmeRepositoryUnicoFalso : IFilmeRepository
{
    private readonly Filme? _filme;

    public FilmeRepositoryUnicoFalso(Filme? filme)
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

    public Task<Pessoa> ObterOuCriarPessoaAsync(int? tmdbPessoaId, string nome, string? imagemUrl) =>
        Task.FromResult(new Pessoa { Nome = nome, ImagemUrl = imagemUrl });

    public Task<bool> UtilizadorViuFilmeAsync(int utilizadorId, int filmeId) =>
        Task.FromResult(false);

    public Task<Avaliacao?> ObterAvaliacaoAsync(int utilizadorId, int filmeId) =>
        Task.FromResult<Avaliacao?>(null);

    public Task AddAvaliacaoAsync(Avaliacao avaliacao) => Task.CompletedTask;

    public Task AddAsync(Filme filme) => Task.CompletedTask;

    public void AtualizarVideo(Filme filme, string? provider, string? key, string? url) { }

    public Task SaveChangesAsync() => Task.CompletedTask;
}
