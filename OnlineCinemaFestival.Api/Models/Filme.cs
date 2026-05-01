namespace OnlineCinemaFestival.Api.Models;

public class Filme
{
    public int Id { get; set; }
    public int TmdbId { get; set; }
    public string Titulo { get; set; } = "";
    public string? Sinopse { get; set; }
    public DateTime DataLancamento { get; set; }
    public string? Genero { get; set; }
    public string? Classificacao { get; set; }
    public string CapaUrl { get; set; } = "";

    public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public int Popularidade => Avaliacoes.Count;
}
