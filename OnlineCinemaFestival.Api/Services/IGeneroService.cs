using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IGeneroService
{
    Task<IEnumerable<GeneroDTO>> ObterTodosAsync();

    Task<GeneroDTO> CriarAsync(CriarGeneroDTO dto);
}
