using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoService
{
    Task<IEnumerable<AcessoReadDTO>> ObterTodosAsync();

    Task<AcessoReadDTO?> ObterPorIdAsync(int id);

    IEnumerable<TipoAcessoReadDTO> GetTiposAcesso();

    Task<AcessoReadDTO> CriarAsync(AcessoCreateDTO dto);

    Task AtualizarAsync(int id, AcessoUpdateDTO dto);

    Task EliminarAsync(int id);
}
