using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class CompraService : ICompraService
{
    private readonly ICompraRepository _compraRepository;

    public CompraService(ICompraRepository compraRepository)
    {
        _compraRepository = compraRepository;
    }

    public async Task<IEnumerable<CompraReadDto>> ObterComprasDoUtilizadorAsync(int utilizadorId)
    {
        var compras = await _compraRepository.GetByUtilizadorIdAsync(utilizadorId);

        return compras.Select(CompraMapper.MapToReadDto);
    }
}
