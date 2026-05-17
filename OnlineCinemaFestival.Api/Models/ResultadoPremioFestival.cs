namespace OnlineCinemaFestival.Api.Models;

public class ResultadoPremioFestival
{
    public int Id { get; set; }

    public int PremioFestivalId { get; set; }
    public PremioFestival PremioFestival { get; set; } = null!;

    public int FilmeIdVencedor { get; set; }
    public Filme FilmeVencedor { get; set; } = null!;

    public int TotalVotos { get; set; }

    public DateTime PublicadoEm { get; set; }

    public int PublicadoPorUtilizadorId { get; set; }
    public Utilizador PublicadoPorUtilizador { get; set; } = null!;
}
