using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class AluguerDigitalCompraItemValidator : ICompraItemValidator
{
    public bool CanHandle(TipoAcesso tipo) => tipo == TipoAcesso.AluguerDigital;

    public void Validar(CompraItemDto item)
    {
        if (!item.FilmeId.HasValue)
        {
            throw new ArgumentException("FilmeId e obrigatorio para AluguerDigital.");
        }
    }
}
