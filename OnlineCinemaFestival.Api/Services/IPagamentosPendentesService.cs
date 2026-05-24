using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IPagamentosPendentesService
{
    Task<List<CompraReadDTO>> ObterDoUtilizadorAsync(int utilizadorId);

    Task<int> ExpirarPendentesAsync(CancellationToken cancellationToken = default);

    Task<CompraReadDTO> ConfirmarPagamentoAsync(int compraId, int utilizadorId);
}
