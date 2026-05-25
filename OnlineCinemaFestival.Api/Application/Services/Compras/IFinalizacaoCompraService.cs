using OnlineCinemaFestival.Api.Application.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IFinalizacaoCompraService
{
    Task<ResultadoFinalizacaoCompraDTO> FinalizarCompraAsync(int utilizadorId, string metodoPagamento);
}
