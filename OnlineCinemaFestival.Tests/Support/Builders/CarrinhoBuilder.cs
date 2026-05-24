using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class CarrinhoBuilder
{
    private int _id = 1;
    private int _utilizadorId = 1;
    private DateTime _dataCriacao = new(2026, 5, 23, 12, 0, 0, DateTimeKind.Utc);
    private readonly List<CarrinhoItem> _itens = new();

    public CarrinhoBuilder ComId(int id) { _id = id; return this; }
    public CarrinhoBuilder DoUtilizador(int utilizadorId) { _utilizadorId = utilizadorId; return this; }
    public CarrinhoBuilder CriadoEm(DateTime data) { _dataCriacao = data; return this; }

    public CarrinhoBuilder ComItem(Acesso acesso, int quantidade = 1, decimal? preco = null)
    {
        _itens.Add(new CarrinhoItem
        {
            AcessoId = acesso.Id,
            Acesso = acesso,
            PrecoUnitario = preco ?? acesso.Preco,
            Quantidade = quantidade,
        });
        return this;
    }

    public Carrinho Build() => new()
    {
        Id = _id,
        UtilizadorId = _utilizadorId,
        DataCriacao = _dataCriacao,
        Itens = _itens,
    };

    public static implicit operator Carrinho(CarrinhoBuilder builder) => builder.Build();
}
