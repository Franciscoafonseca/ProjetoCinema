using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class CompraMapper
{
    public static ResultadoFinalizacaoCompraDTO MapToCheckoutResultadoDTO(
        Compra compra,
        int acessosGerados,
        string mensagem
    )
    {
        var dto = MapToReadDTO(compra);

        return new ResultadoFinalizacaoCompraDTO
        {
            Id = dto.Id,
            Referencia = dto.Referencia,
            UtilizadorId = dto.UtilizadorId,
            ValorTotal = dto.ValorTotal,
            Estado = dto.Estado,
            EstadoNome = dto.EstadoNome,
            CriadaEm = dto.CriadaEm,
            PagaEm = dto.PagaEm,
            Pagamento = dto.Pagamento,
            Itens = dto.Itens,
            AcessosGerados = acessosGerados,
            Mensagem = mensagem,
        };
    }

    public static CompraResumoDTO MapToResumoDTO(Compra compra)
    {
        return new CompraResumoDTO
        {
            Id = compra.Id,
            Referencia = compra.Referencia,
            ValorTotal = compra.ValorTotal,
            EstadoNome = compra.Estado.ToString(),
            CriadaEm = compra.CriadaEm,
            NumeroItens = compra.Itens.Sum(i => i.Quantidade),
        };
    }

    public static CompraReadDTO MapToReadDTO(Compra compra)
    {
        return new CompraReadDTO
        {
            Id = compra.Id,
            Referencia = compra.Referencia,
            UtilizadorId = compra.UtilizadorId,
            ValorTotal = compra.ValorTotal,
            Estado = compra.Estado,
            EstadoNome = compra.Estado.ToString(),
            CriadaEm = compra.CriadaEm,
            PagaEm = compra.PagaEm,
            Pagamento =
                compra.Pagamento == null
                    ? null
                    : new PagamentoReadDTO
                    {
                        Id = compra.Pagamento.Id,
                        Referencia = compra.Pagamento.Referencia,
                        Valor = compra.Pagamento.Valor,
                        Metodo = compra.Pagamento.Metodo,
                        Estado = compra.Pagamento.Estado,
                        EstadoNome = compra.Pagamento.Estado.ToString(),
                        CriadoEm = compra.Pagamento.CriadoEm,
                        ProcessadoEm = compra.Pagamento.ProcessadoEm,
                        Mensagem = compra.Pagamento.Mensagem,
                    },
            Itens = compra
                .Itens.Select(item => new ItemCompraReadDTO
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
