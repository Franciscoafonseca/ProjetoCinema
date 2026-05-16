using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraService
{
    Task<IEnumerable<CompraReadDTO>> ObterComprasDoUtilizadorAsync(int utilizadorId);
}
