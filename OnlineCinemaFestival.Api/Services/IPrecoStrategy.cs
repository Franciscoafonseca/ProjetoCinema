using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IPrecoStrategy
{
    bool CanHandle(TipoAcesso tipo);
    decimal CalcularPreco(CompraItemDto item);
    DateTime? CalcularValidade(CompraItemDto item);
}
