using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ISessaoService
{
    Task<IEnumerable<SessaoReadDto>> GetAllAsync();

    Task<SessaoReadDto?> GetByIdAsync(int id);

    Task<IEnumerable<SessaoReadDto>> GetByFestivalIdAsync(int festivalId);

    Task<IEnumerable<SessaoReadDto>> GetByFilmeIdAsync(int filmeId);

    Task<SessaoReadDto> CreateAsync(SessaoCreateDto dto);

    Task UpdateAsync(int id, SessaoUpdateDto dto);

    Task DeleteAsync(int id);
}
