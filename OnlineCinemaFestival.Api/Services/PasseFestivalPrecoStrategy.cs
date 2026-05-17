using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PasseFestivalPrecoStrategy : IPrecoStrategy
{
    private const decimal PasseDiarioBase = 12.50m;
    private const decimal PasseCompletoBase = 45.00m;
    private const decimal DescontoFestival = 0.10m;

    public bool CanHandle(TipoAcesso tipo) =>
        tipo == TipoAcesso.PasseDiario || tipo == TipoAcesso.PasseCompleto;

    public decimal CalcularPreco(CompraItemDto item)
    {
        var basePrice = item.Tipo == TipoAcesso.PasseCompleto ? PasseCompletoBase : PasseDiarioBase;
        return basePrice * (1 - DescontoFestival);
    }

    public DateTime? CalcularValidade(CompraItemDto item)
    {
        return item.Tipo == TipoAcesso.PasseCompleto
            ? DateTime.UtcNow.AddDays(15)
            : DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);
    }
}
