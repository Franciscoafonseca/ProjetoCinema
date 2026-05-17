namespace OnlineCinemaFestival.Client.Models;

public class CompraItemDto
{
    public int Tipo { get; set; }
    public int? FilmeId { get; set; }
    public int? SessaoId { get; set; }
    public int? FestivalId { get; set; }

    public string FilmeTitulo { get; set; } = string.Empty;
    public string CapaUrl { get; set; } = string.Empty;
}
