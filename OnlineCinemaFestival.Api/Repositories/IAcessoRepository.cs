using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IAcessoRepository
{
    Task<IEnumerable<Acesso>> ObterTodosAsync();

    Task<Acesso?> ObterPorIdAsync(int id);

    Task<Acesso?> GetAtivoParaCarrinhoAsync(
        TipoAcesso tipo,
        int? festivalId,
        int? filmeId,
        int? sessaoId,
        DateTime? dataPasse
    );

    Task AddAsync(Acesso acesso);

    Task AddManyAsync(IEnumerable<Acesso> acessos);

    void Remove(Acesso acesso);

    Task SaveChangesAsync();
}
