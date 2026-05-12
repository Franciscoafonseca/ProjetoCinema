using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IListaPessoalService
{
    Task<IEnumerable<ListaPessoalReadDto>> GetMinhasListasAsync(int utilizadorId);

    Task<ListaPessoalReadDto> CreateAsync(int utilizadorId, ListaPessoalCreateDto dto);

    Task<ListaPessoalItemReadDto> AdicionarFilmeAsync(int utilizadorId, int listaId, int filmeId);

    Task RemoverFilmeAsync(int utilizadorId, int listaId, int filmeId);
}
