using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class BilheteIndividualPrecoStrategy : IPrecoStrategy
{
    public bool CanHandle(TipoAcesso tipo) =>
        tipo == TipoAcesso.BilheteSessao || tipo == TipoAcesso.AluguerDigital;

    public decimal CalcularPreco(CompraItemDto item)
    {
        return item.Tipo == TipoAcesso.AluguerDigital ? 3.99m : 5.00m;
    }

    public DateTime? CalcularValidade(CompraItemDto item)
    {
        return item.Tipo == TipoAcesso.AluguerDigital ? DateTime.UtcNow.AddHours(48) : null;
    }
}
