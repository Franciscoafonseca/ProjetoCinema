using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class PerfilUtilizador
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }

    public Utilizador Utilizador { get; set; } = null!;

    [MaxLength(500)]
    public string Bio { get; set; } = string.Empty;

    [MaxLength(300)]
    public string ProfileImageUrl { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Location { get; set; } = string.Empty;

    public bool IsPublic { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
