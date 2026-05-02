namespace OnlineCinemaFestival.Api.Models;

public class Comunidade
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPublic { get; set; } = true;

    public int? CreatedByUserId { get; set; }
    public Utilizador? CreatedByUser { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ComunidadeMembro> Members { get; set; } = new List<ComunidadeMembro>();

    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
}
