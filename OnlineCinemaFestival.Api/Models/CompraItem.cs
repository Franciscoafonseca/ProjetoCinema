using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class CompraItem
{
    public int Id { get; set; }

    public int CompraId { get; set; }

    public Compra Compra { get; set; } = null!;

    public TipoAcesso TipoAcesso { get; set; }

    public int? FilmeId { get; set; }

    public int? SessaoId { get; set; }

    public decimal PrecoPago { get; set; }

    public DateTime? Validade { get; set; }
}
