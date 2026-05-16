namespace OnlineCinemaFestival.Api.DTOs;

public class ConteudoVisualizacaoDto
{
    public int FilmeId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string PosterUrl { get; set; } = string.Empty;

    public int Ordem { get; set; }

    public string UrlVisualizacao { get; set; } = string.Empty;
}
