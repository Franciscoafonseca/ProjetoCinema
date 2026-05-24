using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IUtilizadorRepository
{
    Task<Utilizador?> ObterPorIdAsync(int id);

    Task<Utilizador?> ObterPorEmailAsync(string email);

    Task<Utilizador?> ObterPorTelefoneAsync(string telefone);

    Task<Utilizador?> ObterComPerfilAsync(int id);

    Task<List<Utilizador>> ObterPerfisPublicosAsync();

    Task AddAsync(Utilizador utilizador);

    Task SaveChangesAsync();
}
