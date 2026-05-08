using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoService
{
    Task<IEnumerable<AcessoReadDto>> GetAllAsync();

    Task<AcessoReadDto?> GetByIdAsync(int id);

    IEnumerable<TipoAcessoReadDto> GetTiposAcesso();

    Task<AcessoReadDto> CreateAsync(AcessoCreateDto dto);

    Task UpdateAsync(int id, AcessoUpdateDto dto);

    Task DeleteAsync(int id);
}
