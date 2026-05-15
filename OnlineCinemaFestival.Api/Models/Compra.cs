namespace OnlineCinemaFestival.Api.Models;

public class Compra
{
    public int Id { get; set; }

    public string Referencia { get; set; } = string.Empty;

    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; } = null!;

    public decimal ValorTotal { get; set; }

    public EstadoCompra Estado { get; set; } = EstadoCompra.Pendente;

    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public DateTime? PagaEm { get; set; }

    public Pagamento? Pagamento { get; set; }

    public ICollection<ItemCompra> Itens { get; set; } = new List<ItemCompra>();
}
