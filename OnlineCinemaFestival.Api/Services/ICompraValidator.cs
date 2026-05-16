using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraValidator
{
    void Validar(CompraRequest request);
}
