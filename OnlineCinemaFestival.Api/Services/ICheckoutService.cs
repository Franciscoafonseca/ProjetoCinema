using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICheckoutService
{
    Task<CompraReadDto> FinalizarCompraAsync(int utilizadorId);
}
