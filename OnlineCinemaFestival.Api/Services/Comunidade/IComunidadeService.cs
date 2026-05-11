using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IComunidadeService
{
    Task<IEnumerable<ComunidadeReadDto>> GetAllComunidadesAsync();
    Task<ComunidadeReadDto?> GetComunidadeByIdAsync(int id);
    Task<ComunidadeReadDto> CreateComunidadeAsync(ComunidadeCreateDto dto, int criadorUserId);
}