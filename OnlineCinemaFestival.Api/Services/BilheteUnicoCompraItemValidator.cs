using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class BilheteSessaoCompraItemValidator : ICompraItemValidator
{
    public bool CanHandle(TipoAcesso tipo) => tipo == TipoAcesso.BilheteSessao;

    public void Validar(CompraItemDto item)
    {
        if (!item.FilmeId.HasValue)
        {
            throw new ArgumentException("FilmeId é obrigatório para bilhete de sessão.");
        }

        if (!item.SessaoId.HasValue)
        {
            throw new ArgumentException("SessaoId é obrigatório para bilhete de sessão.");
        }
    }
}
