namespace OnlineCinemaFestival.Api.Models;

public class VotoPremioFestival
{
    public int Id { get; set; }

    public int PremioFestivalId { get; set; }
    public PremioFestival PremioFestival { get; set; } = null!;

    public int FestivalId { get; set; }
    public Festival Festival { get; set; } = null!;

    public int FilmeId { get; set; }
    public Filme Filme { get; set; } = null!;

    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; } = null!;

    public DateTime DataVoto { get; set; } = DateTime.UtcNow;
}
