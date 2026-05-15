using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class CompraMapper
{
    public static CheckoutResultadoDto MapToCheckoutResultadoDto(
        Compra compra,
        int acessosGerados,
        string mensagem
    )
    {
        var dto = MapToReadDto(compra);

        return new CheckoutResultadoDto
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

    public static CompraResumoDto MapToResumoDto(Compra compra)
    {
        return new CompraResumoDto
        {
            Id = compra.Id,
            Referencia = compra.Referencia,
            ValorTotal = compra.ValorTotal,
            EstadoNome = compra.Estado.ToString(),
            CriadaEm = compra.CriadaEm,
            NumeroItens = compra.Itens.Sum(i => i.Quantidade),
        };
    }

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
            Pagamento =
                compra.Pagamento == null
                    ? null
                    : new PagamentoReadDto
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
