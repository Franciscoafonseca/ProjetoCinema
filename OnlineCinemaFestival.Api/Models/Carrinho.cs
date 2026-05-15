namespace OnlineCinemaFestival.Api.Models;

public class Carrinho
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; } = null!;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? AtualizadoEm { get; set; }

    public ICollection<CarrinhoItem> Itens { get; set; } = new List<CarrinhoItem>();
}
