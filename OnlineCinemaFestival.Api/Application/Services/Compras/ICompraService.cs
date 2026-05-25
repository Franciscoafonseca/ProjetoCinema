using OnlineCinemaFestival.Api.Application.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraService
{
    Task<IEnumerable<CompraReadDTO>> ObterComprasDoUtilizadorAsync(int utilizadorId);

    Task<IEnumerable<CompraHistoricoReadDto>> ObterHistoricoDoUtilizadorAsync(int utilizadorId);
}
