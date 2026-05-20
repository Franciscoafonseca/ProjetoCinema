using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ISessaoService
{
    Task<IEnumerable<SessaoReadDTO>> ObterTodosAsync();

    Task<SessaoReadDTO?> ObterPorIdAsync(int id);

    Task<IEnumerable<SessaoReadDTO>> ObterPorFestivalIdAsync(int festivalId);

    Task<IEnumerable<SessaoReadDTO>> ObterPorFilmeIdAsync(int filmeId);

    Task<IEnumerable<SessaoReadDTO>> ObterDisponiveisAsync();

    Task<SessaoEstadoReadDTO> ObterEstadoAsync(int id);

    Task<SessaoReadDTO> CriarAsync(SessaoCreateDTO dto);

    Task AtualizarAsync(int id, SessaoUpdateDTO dto);

    Task EliminarAsync(int id);
}
