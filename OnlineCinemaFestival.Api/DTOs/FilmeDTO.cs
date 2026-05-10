namespace OnlineCinemaFestival.Api.DTOs;

public class FilmeDto
{
    public int Id { get; set; }

    public int? TmdbId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Sinopse { get; set; } = string.Empty;

    public DateTime? DataLancamento { get; set; }

    public string Genero { get; set; } = string.Empty;

    public string Realizador { get; set; } = string.Empty;

    public string UrlCartaz { get; set; } = string.Empty;

    public string TrailerUrl { get; set; } = string.Empty;

    public double Popularidade { get; set; }

    public double ClassificacaoMedia { get; set; }

    public decimal Preco { get; set; }
}
