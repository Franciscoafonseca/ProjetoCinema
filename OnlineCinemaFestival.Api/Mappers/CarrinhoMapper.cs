using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class CarrinhoMapper
{
    public static CarrinhoReadDTO MapToReadDTO(Carrinho carrinho)
    {
        var itens = carrinho.Itens.OrderBy(i => i.DataAdicao).Select(MapItemToReadDTO).ToList();

        return new CarrinhoReadDTO
        {
            Id = carrinho.Id,
            UtilizadorId = carrinho.UtilizadorId,
            Itens = itens,
            Total = itens.Sum(i => i.Subtotal),
        };
    }

    private static ItemCarrinhoReadDTO MapItemToReadDTO(CarrinhoItem item)
    {
        var primeiroFilmeSessao = item
            .Acesso.Sessao?.FilmesDaSessao.OrderBy(sf => sf.Ordem)
            .FirstOrDefault();

        return new ItemCarrinhoReadDTO
        {
            Id = item.Id,
            AcessoId = item.AcessoId,
            NomeAcesso = item.Acesso.Nome,
            TipoAcesso = item.Acesso.Tipo,
            TipoAcessoNome = item.Acesso.Tipo.ToString(),
            SessaoId = item.Acesso.SessaoId,
            FestivalId = item.Acesso.FestivalId ?? item.Acesso.Sessao?.FestivalId,
            NomeFestival =
                item.Acesso.Festival?.Name ?? item.Acesso.Sessao?.Festival?.Name ?? string.Empty,
            FilmeId = item.Acesso.FilmeId ?? primeiroFilmeSessao?.FilmeId,
            TituloFilme =
                item.Acesso.Filme?.Titulo ?? primeiroFilmeSessao?.Filme?.Titulo ?? string.Empty,
            InicioSessao = item.Acesso.Sessao?.Inicio,
            FimSessao = item.Acesso.Sessao?.Fim,
            DataAcesso = item.Acesso.DataAcesso,
            DuracaoHoras = item.Acesso.DuracaoHoras,
            PrecoUnitario = item.PrecoUnitario,
            Quantidade = item.Quantidade,
            Subtotal = item.PrecoUnitario * item.Quantidade,
            DataAdicao = item.DataAdicao,
        };
    }
}
