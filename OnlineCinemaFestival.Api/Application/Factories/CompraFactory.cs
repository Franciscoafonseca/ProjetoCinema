using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public sealed class CompraFactory : ICompraFactory
{
    private readonly IGeradorReferenciaCompra _geradorReferenciaCompra;
    private readonly TimeProvider _timeProvider;

    public CompraFactory(IGeradorReferenciaCompra geradorReferenciaCompra, TimeProvider timeProvider)
    {
        _geradorReferenciaCompra = geradorReferenciaCompra;
        _timeProvider = timeProvider;
    }

    public Compra Criar(int utilizadorId, Carrinho carrinho)
    {
        var agora = _timeProvider.GetUtcNow().UtcDateTime;

        var compra = new Compra
        {
            UtilizadorId = utilizadorId,
            Referencia = _geradorReferenciaCompra.Gerar(),
            CriadaEm = agora,
            Estado = EstadoCompra.Pendente,
            Itens = carrinho.Itens.Select(CriarItemCompra).ToList(),
        };

        compra.ValorTotal = compra.Itens.Sum(i => i.Subtotal);

        return compra;
    }

    private static ItemCompra CriarItemCompra(CarrinhoItem item)
    {
        return new ItemCompra
        {
            AcessoId = item.AcessoId,
            Acesso = item.Acesso,
            NomeAcesso = item.Acesso.Nome,
            TipoAcesso = item.Acesso.Tipo,
            PrecoUnitario = item.PrecoUnitario,
            Quantidade = item.Quantidade,
            Subtotal = item.PrecoUnitario * item.Quantidade,
        };
    }
}
