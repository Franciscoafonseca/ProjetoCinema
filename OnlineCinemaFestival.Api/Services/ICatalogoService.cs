using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICatalogoService
{
    Task<IEnumerable<FilmeReadDTO>> ObterCatalogoAsync(CatalogoQueryDTO query);

    Task<IEnumerable<FilmeReadDTO>> ObterFilmesPorFestivalAsync(
        int festivalId,
        CatalogoQueryDTO query
    );

    Task<FilmeReadDTO?> ObterDetalhesFilmeAsync(int filmeId);
}
