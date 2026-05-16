using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface ISessaoRepository
{
    Task<IEnumerable<Sessao>> ObterTodosAsync();

    Task<Sessao?> ObterPorIdAsync(int id);

    Task<IEnumerable<Sessao>> ObterPorFestivalIdAsync(int festivalId);

    Task<IEnumerable<Sessao>> ObterPorFilmeIdAsync(int filmeId);

    Task<IEnumerable<Sessao>> ObterDisponiveisAsync(DateTime dataAtual);

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
