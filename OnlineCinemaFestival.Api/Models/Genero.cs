using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class Genero
{
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UtilizadorGeneroFavorito> UtilizadoresFavoritos { get; set; } =
        new List<UtilizadorGeneroFavorito>();
}
