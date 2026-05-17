using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraItemValidator
{
    bool CanHandle(TipoAcesso tipo);
    void Validar(CompraItemDto item);
}
