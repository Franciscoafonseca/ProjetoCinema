namespace OnlineCinemaFestival.Api.Models;

public class ItemCompra
{
    public int Id { get; set; }

    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;

    public int AcessoId { get; set; }
    public Acesso Acesso { get; set; } = null!;

    public string NomeAcesso { get; set; } = string.Empty;

    public TipoAcesso TipoAcesso { get; set; }

    public decimal PrecoUnitario { get; set; }

    public int Quantidade { get; set; } = 1;

    public decimal Subtotal { get; set; }
}
