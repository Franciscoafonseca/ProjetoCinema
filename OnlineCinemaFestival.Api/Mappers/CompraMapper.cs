using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class CompraMapper
{
    public static CompraReadDto MapToReadDto(Compra compra)
    {
        return new CompraReadDto
        {
            Id = compra.Id,
            Referencia = compra.Referencia,
            UtilizadorId = compra.UtilizadorId,
            ValorTotal = compra.ValorTotal,
            Estado = compra.Estado,
            EstadoNome = compra.Estado.ToString(),
            CriadaEm = compra.CriadaEm,
            PagaEm = compra.PagaEm,
            Itens = compra
                .Itens.Select(item => new ItemCompraReadDto
                {
                    Id = item.Id,
                    AcessoId = item.AcessoId,
                    NomeAcesso = item.NomeAcesso,
                    TipoAcesso = item.TipoAcesso,
                    TipoAcessoNome = item.TipoAcesso.ToString(),
                    PrecoUnitario = item.PrecoUnitario,
                    Quantidade = item.Quantidade,
                    Subtotal = item.Subtotal,
                })
                .ToList(),
        };
    }
}
