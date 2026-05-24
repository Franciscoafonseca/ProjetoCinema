using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class CompraBuilder
{
    private int _id = 1;
    private string _referencia = "CMP-TESTE-0001";
    private int _utilizadorId = 1;
    private decimal _valorTotal = 0m;
    private EstadoCompra _estado = EstadoCompra.Pago;
    private DateTime _criadaEm = new(2026, 5, 23, 12, 0, 0, DateTimeKind.Utc);
    private DateTime? _pagaEm;
    private Pagamento? _pagamento;
    private readonly List<ItemCompra> _itens = new();

    public CompraBuilder ComId(int id) { _id = id; return this; }
    public CompraBuilder ComReferencia(string referencia) { _referencia = referencia; return this; }
    public CompraBuilder DoUtilizador(int utilizadorId) { _utilizadorId = utilizadorId; return this; }
    public CompraBuilder ComValorTotal(decimal valor) { _valorTotal = valor; return this; }
    public CompraBuilder NoEstado(EstadoCompra estado) { _estado = estado; return this; }
    public CompraBuilder CriadaEm(DateTime data) { _criadaEm = data; return this; }
    public CompraBuilder PagaEm(DateTime data) { _pagaEm = data; return this; }
    public CompraBuilder ComPagamento(Pagamento pagamento) { _pagamento = pagamento; return this; }

    public CompraBuilder ComItem(Acesso acesso, int quantidade = 1, decimal? preco = null)
    {
        var precoUsado = preco ?? acesso.Preco;
        _itens.Add(new ItemCompra
        {
            AcessoId = acesso.Id,
            Acesso = acesso,
            NomeAcesso = acesso.Nome,
            TipoAcesso = acesso.Tipo,
            PrecoUnitario = precoUsado,
            Quantidade = quantidade,
            Subtotal = precoUsado * quantidade,
        });
        return this;
    }

    public Compra Build()
    {
        var compra = new Compra
        {
            Id = _id,
            Referencia = _referencia,
            UtilizadorId = _utilizadorId,
            ValorTotal = _valorTotal == 0m && _itens.Count > 0 ? _itens.Sum(i => i.Subtotal) : _valorTotal,
            Estado = _estado,
            CriadaEm = _criadaEm,
            PagaEm = _pagaEm,
            Pagamento = _pagamento,
            Itens = _itens,
        };

        if (_pagamento != null)
            _pagamento.Compra = compra;

        return compra;
    }

    public static implicit operator Compra(CompraBuilder builder) => builder.Build();
}
