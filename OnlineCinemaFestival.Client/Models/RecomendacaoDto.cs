namespace OnlineCinemaFestival.Client.Models;

public class FilmeRecomendadoDTO
{
    public FilmeDTO Filme { get; set; } = new();

    public string Motivo { get; set; } = string.Empty;

    public decimal Pontuacao { get; set; }
}
