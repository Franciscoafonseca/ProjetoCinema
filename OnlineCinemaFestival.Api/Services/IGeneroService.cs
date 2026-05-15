using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IGeneroService
{
    Task<IEnumerable<GeneroDto>> GetAllAsync();

    Task<GeneroDto> CreateAsync(CriarGeneroDto dto);
}
