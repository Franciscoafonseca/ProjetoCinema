using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface ISessaoRepository
{
    Task<IEnumerable<Sessao>> GetAllAsync();

    Task<Sessao?> GetByIdAsync(int id);

    Task<IEnumerable<Sessao>> GetByFestivalIdAsync(int festivalId);

    Task<IEnumerable<Sessao>> GetByFilmeIdAsync(int filmeId);

    Task<IEnumerable<Sessao>> GetDisponiveisAsync(DateTime dataAtual);

    Task<bool> HasOverlapAsync(
        int festivalId,
        IEnumerable<int> filmeIds,
        DateTime inicio,
        DateTime fim,
        int? ignoreSessaoId = null
    );

    Task<bool> HasAcessosAssociadosAsync(int sessaoId);

    Task AddAsync(Sessao sessao);

    void Remove(Sessao sessao);

    Task SaveChangesAsync();
}
