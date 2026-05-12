using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class CarrinhoMapper
{
    public static CarrinhoReadDto MapToReadDto(Carrinho carrinho)
    {
        var itens = carrinho.Itens.OrderBy(i => i.DataAdicao).Select(MapItemToReadDto).ToList();

        return new CarrinhoReadDto
        {
            Id = carrinho.Id,
            UtilizadorId = carrinho.UtilizadorId,
            Itens = itens,
            Total = itens.Sum(i => i.Subtotal),
        };
    }

    private static ItemCarrinhoReadDto MapItemToReadDto(ItemCarrinho item)
    {
        return new ItemCarrinhoReadDto
        {
            Id = item.Id,
            AcessoId = item.AcessoId,
            NomeAcesso = item.Acesso.Nome,
            TipoAcesso = item.Acesso.Tipo,
            TipoAcessoNome = item.Acesso.Tipo.ToString(),
            PrecoUnitario = item.PrecoUnitario,
            Quantidade = item.Quantidade,
            Subtotal = item.PrecoUnitario * item.Quantidade,
        };
    }
}
