namespace OnlineCinemaFestival.Api.Models;

public class ItemCarrinho
{
    public int Id { get; set; }

    public int CarrinhoId { get; set; }
    public Carrinho Carrinho { get; set; } = null!;

    public int AcessoId { get; set; }
    public Acesso Acesso { get; set; } = null!;

    public decimal PrecoUnitario { get; set; }

    public int Quantidade { get; set; } = 1;

    public DateTime DataAdicao { get; set; } = DateTime.UtcNow;
}
