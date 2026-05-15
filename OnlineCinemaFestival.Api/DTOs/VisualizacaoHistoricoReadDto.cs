namespace OnlineCinemaFestival.Api.DTOs;

public class VisualizacaoHistoricoReadDto
{
    public int Id { get; set; }

    public int FilmeId { get; set; }

    public string FilmeTitulo { get; set; } = string.Empty;

    public int? SessaoId { get; set; }

    public int? FestivalId { get; set; }

    public string FestivalNome { get; set; } = string.Empty;

    public string TipoConteudo { get; set; } = string.Empty;

    public string? UrlVisualizacao { get; set; }

    public DateTime VisualizadoEm { get; set; }
}
