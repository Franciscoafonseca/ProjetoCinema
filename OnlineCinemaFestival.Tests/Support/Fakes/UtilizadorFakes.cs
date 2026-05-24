using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Tests.Support.Fakes;

public sealed class UtilizadorRepositoryFalso : IUtilizadorRepository
{
    private readonly Dictionary<int, Utilizador> _utilizadores;

    public UtilizadorRepositoryFalso(params Utilizador[] utilizadores)
    {
        _utilizadores = utilizadores.ToDictionary(u => u.Id);
    }

    public Task<Utilizador?> ObterPorIdAsync(int id) =>
        Task.FromResult(_utilizadores.GetValueOrDefault(id));

    public Task<Utilizador?> ObterPorEmailAsync(string email) =>
        Task.FromResult(_utilizadores.Values.FirstOrDefault(u => u.Email == email));

    public Task<Utilizador?> ObterPorTelefoneAsync(string telefone) =>
        Task.FromResult(_utilizadores.Values.FirstOrDefault(u => u.PhoneNumber == telefone));

    public Task<Utilizador?> ObterComPerfilAsync(int id) =>
        Task.FromResult(_utilizadores.GetValueOrDefault(id));

    public Task<List<Utilizador>> ObterPerfisPublicosAsync() =>
        Task.FromResult(_utilizadores.Values.Where(u => u.Perfil?.IsPublic == true).ToList());

    public Task AddAsync(Utilizador utilizador)
    {
        _utilizadores[utilizador.Id] = utilizador;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}

public sealed class GeneroRepositoryFalso : IGeneroRepository
{
    public Task<IEnumerable<Genero>> ObterTodosAsync() =>
        Task.FromResult<IEnumerable<Genero>>(Array.Empty<Genero>());

    public Task<Genero?> ObterPorIdAsync(int id) => Task.FromResult<Genero?>(null);

    public Task AddAsync(Genero genero) => Task.CompletedTask;

    public Task SaveChangesAsync() => Task.CompletedTask;

    public Task<List<Genero>> ObterPorIdsAsync(IEnumerable<int> ids) =>
        Task.FromResult(new List<Genero>());
}
