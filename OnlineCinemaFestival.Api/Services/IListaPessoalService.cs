using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IListaPessoalService
{
    Task<IEnumerable<ListaPessoalReadDTO>> ObterMinhasListasAsync(int utilizadorId);

    Task<ListaPessoalReadDTO> CriarAsync(int utilizadorId, ListaPessoalCreateDTO dto);

    Task<ListaPessoalItemReadDTO> AdicionarFilmeAsync(int utilizadorId, int listaId, int filmeId);

    Task RemoverFilmeAsync(int utilizadorId, int listaId, int filmeId);

    Task RemoverListaAsync(int utilizadorId, int listaId);
}
