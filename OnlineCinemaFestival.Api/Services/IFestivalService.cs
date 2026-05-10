using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IFestivalService
{
    Task<IEnumerable<FestivalReadDto>> GetAllAsync();

    Task<FestivalReadDto?> GetByIdAsync(int id);

    Task<FestivalReadDto> CreateAsync(FestivalCreateDto dto);

    Task UpdateAsync(int id, FestivalUpdateDto dto);

    Task DeleteAsync(int id);
}
