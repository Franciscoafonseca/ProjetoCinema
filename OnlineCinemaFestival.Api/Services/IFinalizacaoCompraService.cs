using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Configuracao;

namespace OnlineCinemaFestival.Api.Services;

public interface IFinalizacaoCompraService
{
    Task<ResultadoFinalizacaoCompraDTO> FinalizarCompraAsync(
        int utilizadorId,
        string metodoPagamento = MetodosPagamento.CartaoCredito
    );
}
