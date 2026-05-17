namespace OnlineCinemaFestival.Api.Models;

public class Filme
{
    public int Id { get; set; }
    public int TmdbId { get; set; }
    public string Titulo { get; set; } = "";
    public string? TituloOriginal { get; set; }
    public string? Sinopse { get; set; }
    public DateTime DataLancamento { get; set; }
    public int? DuracaoMinutos { get; set; }
    public string? Genero { get; set; }
    public string? Classificacao { get; set; }
    public double? AvaliacaoTmdb { get; set; }
    public string CapaUrl { get; set; } = "";
    public string? TrailerUrl { get; set; }
    public string? VideoProvider { get; set; }
    public string? VideoKey { get; set; }
    public string? VideoUrl { get; set; }
    public int? DuracaoVideoSegundos { get; set; }
    public string? ConteudoLocalPath { get; set; }
    public string? Realizador { get; set; }
    public string? AtoresPrincipais { get; set; }
    public string? TmdbReviewsJson { get; set; }
    public string? Premios { get; set; }

    public ICollection<FilmeGenero> FilmeGeneros { get; set; } = new List<FilmeGenero>();

    public ICollection<FilmePessoa> PessoasDoFilme { get; set; } = new List<FilmePessoa>();

    public ICollection<FestivalFilme> FestivalFilmes { get; set; } = new List<FestivalFilme>();

    public ICollection<SessaoFilme> SessoesDoFilme { get; set; } = new List<SessaoFilme>();

    public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();

    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public ICollection<ListaPessoalItem> ListaPessoalItems { get; set; } =
        new List<ListaPessoalItem>();

    public ICollection<Acesso> Acessos { get; set; } = new List<Acesso>();

    public ICollection<AcessoUtilizador> AcessosUtilizador { get; set; } =
        new List<AcessoUtilizador>();

    public ICollection<Visualizacao> Visualizacoes { get; set; } = new List<Visualizacao>();

    public ICollection<VotoPremioFestival> VotosPremiosFestival { get; set; } =
        new List<VotoPremioFestival>();

    public ICollection<ResultadoPremioFestival> ResultadosPremiosFestival { get; set; } =
        new List<ResultadoPremioFestival>();

    public int Popularidade => Avaliacoes.Count;
}
