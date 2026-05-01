using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class Comunidade
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(700)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(300)]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPublic { get; set; } = true;

    public int CreatedByUserId { get; set; }

    public Utilizador CreatedByUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ComunidadeMembro> Members { get; set; } = new List<ComunidadeMembro>();
}
