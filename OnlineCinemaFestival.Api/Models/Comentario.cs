namespace OnlineCinemaFestival.Api.Models;

public class Comentario
{
    public int Id { get; set; }

    public int FilmeId { get; set; }

    public Filme Filme { get; set; } = null!;

    public int? UsuarioId { get; set; }

    public Utilizador? Usuario { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTime Data { get; set; } = DateTime.UtcNow;

    public bool Reportado { get; set; } = false;

    public bool Visivel { get; set; } = true;
}
