using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class ListaPessoal
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }

    public Utilizador Utilizador { get; set; } = null!;

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public TipoListaPessoal Tipo { get; set; } = TipoListaPessoal.Custom;

    public bool IsPublic { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ListaPessoalItem> Items { get; set; } = new List<ListaPessoalItem>();
}
