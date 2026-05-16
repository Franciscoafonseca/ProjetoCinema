using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class BilheteUnicoCompraItemValidator : ICompraItemValidator
{
    public bool CanHandle(TipoAcesso tipo) => tipo == TipoAcesso.BilheteUnico;

    public void Validar(CompraItemDto item)
    {
        if (!item.FilmeId.HasValue)
        {
            throw new ArgumentException("FilmeId e obrigatorio para BilheteUnico.");
        }

        if (!item.SessaoId.HasValue)
        {
            throw new ArgumentException("SessaoId e obrigatorio para BilheteUnico.");
        }
    }
}
