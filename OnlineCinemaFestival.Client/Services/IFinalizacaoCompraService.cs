using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public interface IFinalizacaoCompraService
{
    Task<ResultadoFinalizacaoCompraDTO> FinalizarCompraAsync(string metodoPagamento);
}
