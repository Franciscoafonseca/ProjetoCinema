namespace OnlineCinemaFestival.Client.Models;

public class FilmeDto
{
    public int Id { get; set; }

    public int TmdbId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string? Sinopse { get; set; }

    public DateTime DataLancamento { get; set; }

    public string? Genero { get; set; }

    public string? Classificacao { get; set; }

    public string CapaUrl { get; set; } = string.Empty;

    public int Popularidade { get; set; }

    // Compatibilidade com páginas antigas tuas que usam UrlCartaz
    public string UrlCartaz => CapaUrl;
}
