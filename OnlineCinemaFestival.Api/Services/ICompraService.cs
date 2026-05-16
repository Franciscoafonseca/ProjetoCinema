using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraService
{
    Task<CompraResultado> FinalizarProcessoCompra(string utilizadorId, List<CompraItemDto> itensCarrinho);
}
