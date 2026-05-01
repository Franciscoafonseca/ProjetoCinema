namespace OnlineCinemaFestival.Api.Models;

public class Avaliacao
{
    public int Id { get; set; }

    public int FilmeId { get; set; }

    public Filme Filme { get; set; } = null!;

    public int UsuarioId { get; set; }

    public Utilizador Usuario { get; set; } = null!;

    public int Pontuacao { get; set; }

    public DateTime Data { get; set; } = DateTime.UtcNow;
}
