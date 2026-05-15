using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICheckoutService
{
    Task<CheckoutResultadoDto> FinalizarCompraAsync(int utilizadorId);
}
