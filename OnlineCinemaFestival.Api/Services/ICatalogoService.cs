using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICatalogoService
{
    Task<IEnumerable<FilmeReadDto>> GetCatalogoAsync(CatalogoQueryDto query);

    Task<IEnumerable<FilmeReadDto>> GetFilmesByFestivalAsync(
        int festivalId,
        CatalogoQueryDto query
    );

    Task<FilmeReadDto?> GetFilmeDetalhesAsync(int filmeId);
}
