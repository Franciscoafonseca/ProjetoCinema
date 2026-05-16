using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IFestivalService
{
    Task<IEnumerable<FestivalReadDTO>> ObterTodosAsync();

    Task<FestivalReadDTO?> ObterPorIdAsync(int id);

    Task<FestivalReadDTO> CriarAsync(FestivalCreateDTO dto);

    Task AtualizarAsync(int id, FestivalUpdateDTO dto);

    Task EliminarAsync(int id);
}
