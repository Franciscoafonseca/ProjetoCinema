using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class Compra
{
    public int Id { get; set; }

    [Required]
    public string UtilizadorId { get; set; } = string.Empty;

    public DateTime Data { get; set; } = DateTime.UtcNow;

    public decimal Total { get; set; }

    public int PontosGanhos { get; set; }

    public ICollection<CompraItem> Itens { get; set; } = new List<CompraItem>();
}
