using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Tests.Listas;

/// <summary>
/// Testa que listas pessoais impedem duplicados de nomes e de filmes,
/// e que listas predefinidas não podem ser apagadas.
/// </summary>
public class ListaSemDuplicadosTests
{
    // ── Criação de lista ──────────────────────────────────────────────────────

    [Fact]
    public async Task CriarLista_NomeValido_Cria()
    {
        var (service, _) = CriarServico();

        var lista = await service.CriarAsync(1, new ListaPessoalCreateDTO { Name = "Os meus favoritos" });

        Assert.Equal("Os meus favoritos", lista.Name);
    }

    [Fact]
    public async Task CriarLista_NomeDuplicado_Rejeita()
    {
        var (service, _) = CriarServico();
        await service.CriarAsync(1, new ListaPessoalCreateDTO { Name = "Séries para ver" });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CriarAsync(1, new ListaPessoalCreateDTO { Name = "Séries para ver" })
        );
    }

    [Fact]
    public async Task CriarLista_NomeVazio_Rejeita()
    {
        var (service, _) = CriarServico();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CriarAsync(1, new ListaPessoalCreateDTO { Name = "   " })
        );
    }

    [Fact]
    public async Task CriarLista_NomeMuitoCurto_Rejeita()
    {
        var (service, _) = CriarServico();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CriarAsync(1, new ListaPessoalCreateDTO { Name = "AB" })
        );
    }

    [Fact]
    public async Task CriarLista_NomeMuitoLongo_Rejeita()
    {
        var (service, _) = CriarServico();
        var nomeLongo = new string('A', 51);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CriarAsync(1, new ListaPessoalCreateDTO { Name = nomeLongo })
        );
    }

    // ── Adição de filme ───────────────────────────────────────────────────────

    [Fact]
    public async Task AdicionarFilme_PrimeiraVez_Adiciona()
    {
        var (service, repo) = CriarServico();
        await repo.AddAsync(CriarLista(1, 10));

        await service.AdicionarFilmeAsync(utilizadorId: 1, listaId: 10, filmeId: 99);

        var item = await repo.ObterItemAsync(10, 99);
        Assert.NotNull(item);
    }

    [Fact]
    public async Task AdicionarFilme_JaExistente_Rejeita()
    {
        var (service, repo) = CriarServico();
        await repo.AddAsync(CriarLista(1, 10));
        await service.AdicionarFilmeAsync(utilizadorId: 1, listaId: 10, filmeId: 99);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AdicionarFilmeAsync(utilizadorId: 1, listaId: 10, filmeId: 99)
        );
    }

    [Fact]
    public async Task AdicionarFilme_ListaDeOutroUtilizador_Rejeita()
    {
        var (service, repo) = CriarServico();
        await repo.AddAsync(CriarLista(utilizadorId: 1, listaId: 10));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.AdicionarFilmeAsync(utilizadorId: 2, listaId: 10, filmeId: 99)
        );
    }

    [Fact]
    public async Task AdicionarFilme_FilmeNaoExiste_Rejeita()
    {
        var (service, repo) = CriarServico(filmeExiste: false);
        await repo.AddAsync(CriarLista(1, 10));

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AdicionarFilmeAsync(utilizadorId: 1, listaId: 10, filmeId: 999)
        );
    }

    // ── Remoção de lista ──────────────────────────────────────────────────────

    [Fact]
    public async Task RemoverLista_Personalizada_Remove()
    {
        var (service, repo) = CriarServico();
        await repo.AddAsync(CriarLista(1, 10, TipoListaPessoal.Custom));

        await service.RemoverListaAsync(utilizadorId: 1, listaId: 10);

        Assert.Null(await repo.ObterPorIdAsync(10));
    }

    [Fact]
    public async Task RemoverLista_Predefinida_Rejeita()
    {
        var (service, repo) = CriarServico();
        await repo.AddAsync(CriarLista(1, 10, TipoListaPessoal.Watchlist));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RemoverListaAsync(utilizadorId: 1, listaId: 10)
        );
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static (ListaPessoalService Service, ListaPessoalRepositoryFalso Repo) CriarServico(
        bool filmeExiste = true
    )
    {
        var repo = new ListaPessoalRepositoryFalso(filmeExiste);
        var service = new ListaPessoalService(repo, Array.Empty<IListaPessoalObserver>());
        return (service, repo);
    }

    private static ListaPessoal CriarLista(
        int utilizadorId,
        int listaId,
        TipoListaPessoal tipo = TipoListaPessoal.Custom
    ) =>
        new()
        {
            Id = listaId,
            UtilizadorId = utilizadorId,
            Name = $"Lista-{listaId}",
            Tipo = tipo,
        };

    // ── Repositório falso ─────────────────────────────────────────────────────

    private sealed class ListaPessoalRepositoryFalso : IListaPessoalRepository
    {
        private readonly bool _filmeExiste;
        private readonly List<ListaPessoal> _listas = [];
        private readonly List<ListaPessoalItem> _itens = [];
        private int _seq;

        public ListaPessoalRepositoryFalso(bool filmeExiste = true)
        {
            _filmeExiste = filmeExiste;
        }

        public Task<IEnumerable<ListaPessoal>> GetByUtilizadorAsync(int utilizadorId) =>
            Task.FromResult<IEnumerable<ListaPessoal>>(
                _listas.Where(l => l.UtilizadorId == utilizadorId)
            );

        public Task<bool> ExisteNomeParaUtilizadorAsync(int utilizadorId, string nome) =>
            Task.FromResult(
                _listas.Any(l => l.UtilizadorId == utilizadorId && l.Name == nome)
            );

        public Task<ListaPessoal?> ObterPorIdAsync(int id) =>
            Task.FromResult(_listas.FirstOrDefault(l => l.Id == id));

        public Task AddAsync(ListaPessoal lista)
        {
            if (lista.Id == 0)
                lista.Id = ++_seq;
            _listas.Add(lista);
            return Task.CompletedTask;
        }

        public Task<bool> FilmeExisteAsync(int filmeId) => Task.FromResult(_filmeExiste);

        public Task<ListaPessoalItem?> ObterItemAsync(int listaId, int filmeId) =>
            Task.FromResult(
                _itens.FirstOrDefault(i => i.ListaPessoalId == listaId && i.FilmeId == filmeId)
            );

        public Task AddItemAsync(ListaPessoalItem item)
        {
            _itens.Add(item);
            return Task.CompletedTask;
        }

        public void RemoveItem(ListaPessoalItem item) => _itens.Remove(item);

        public void Remove(ListaPessoal lista) => _listas.Remove(lista);

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
