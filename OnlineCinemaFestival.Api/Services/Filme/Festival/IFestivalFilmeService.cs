using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IFestivalFilmeService
{
    Task<FestivalFilmeReadDTO> AssociarFilmeAsync(
        int festivalId,
        AssociarFilmeFestivalDTO dto
    );

    Task RemoverFilmeAsync(int festivalId, int filmeId);

    Task<IEnumerable<FilmeReadDTO>> ObterFilmesPorFestivalAsync(int festivalId);

    Task<IEnumerable<FestivalFilmeReadDTO>> ObterAssociacoesPorFestivalAsync(int festivalId);
}
