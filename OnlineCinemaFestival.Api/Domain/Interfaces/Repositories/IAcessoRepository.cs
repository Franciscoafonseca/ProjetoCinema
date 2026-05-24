using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IAcessoRepository
{
    Task<IEnumerable<Acesso>> ObterTodosAsync();

    Task<Acesso?> ObterPorIdAsync(int id);

    Task<Acesso?> ObterBilheteSessaoAtivoAsync(int sessaoId);

    Task<Acesso?> ObterPasseDiarioAtivoAsync(int festivalId, DateTime dataAcesso);

    Task<Acesso?> ObterPasseCompletoAtivoAsync(int festivalId);

    Task<Acesso?> ObterAluguerDigitalAtivoAsync(int filmeId);

    Task AddAsync(Acesso acesso);

    Task AddManyAsync(IEnumerable<Acesso> acessos);

    void Remove(Acesso acesso);

    Task SaveChangesAsync();
}
