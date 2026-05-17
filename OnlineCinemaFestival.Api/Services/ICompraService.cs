using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraService
{
    Task<IEnumerable<CompraReadDTO>> ObterComprasDoUtilizadorAsync(int utilizadorId);

    Task<IEnumerable<CompraHistoricoReadDto>> ObterHistoricoDoUtilizadorAsync(int utilizadorId);

    Task<CompraResultado> FinalizarProcessoCompraAsync(
        int utilizadorId,
        List<CompraItemDto> itensCarrinho
    );
}
