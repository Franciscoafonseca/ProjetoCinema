namespace OnlineCinemaFestival.Api.DTOs;

public class TmdbFilmeDto
{
    public int TmdbId { get; set; }
    public string Titulo { get; set; } = "";
    public string? Sinopse { get; set; }
    public string? Genero { get; set; }
    public string? Classificacao { get; set; }
    public string CapaUrl { get; set; } = "";
    public DateTime DataLancamento { get; set; }
}
