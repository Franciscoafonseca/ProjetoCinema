namespace OnlineCinemaFestival.Api.Models;

public class Comentario
{
    public int Id { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public bool Reportado { get; set; } = false;

    public bool Visivel { get; set; } = true;

    public int UsuarioId { get; set; }
    public Utilizador Usuario { get; set; } = null!;

    public int ComunidadeId { get; set; }
    public Comunidade Comunidade { get; set; } = null!;
}
