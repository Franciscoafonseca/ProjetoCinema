using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IFestivalFilmeService
{
    Task AssociarFilmeAsync(int festivalId, int filmeId);

    Task RemoverFilmeAsync(int festivalId, int filmeId);

    Task<IEnumerable<FilmeReadDto>> GetFilmesByFestivalAsync(int festivalId);
}
