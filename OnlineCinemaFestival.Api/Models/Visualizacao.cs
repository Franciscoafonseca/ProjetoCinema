namespace OnlineCinemaFestival.Api.Models;

public class Visualizacao
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; } = null!;

    public int FilmeId { get; set; }
    public Filme Filme { get; set; } = null!;

    public int? SessaoId { get; set; }
    public Sessao? Sessao { get; set; }

    public int? FestivalId { get; set; }
    public Festival? Festival { get; set; }

    public string TipoConteudo { get; set; } = string.Empty;

    public string? UrlVisualizacao { get; set; }

    public DateTime VisualizadoEm { get; set; } = DateTime.UtcNow;
}
