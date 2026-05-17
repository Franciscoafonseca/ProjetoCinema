namespace OnlineCinemaFestival.Api.Models;

public class Pessoa
{
    public int Id { get; set; }

    public int? TmdbPessoaId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? ImagemUrl { get; set; }

    public ICollection<FilmePessoa> Filmes { get; set; } = new List<FilmePessoa>();
}
