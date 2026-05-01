namespace OnlineCinemaFestival.Api.Models;

public class UtilizadorGeneroFavorito
{
    public int UtilizadorId { get; set; }

    public Utilizador Utilizador { get; set; } = null!;

    public int GeneroId { get; set; }

    public Genero Genero { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
