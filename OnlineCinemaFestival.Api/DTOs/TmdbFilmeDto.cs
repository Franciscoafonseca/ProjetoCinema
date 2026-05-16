namespace OnlineCinemaFestival.Api.DTOs;

public class TmdbFilmeDTO
{
    public int TmdbId { get; set; }
    public string Titulo { get; set; } = "";
    public string? TituloOriginal { get; set; }
    public string? Sinopse { get; set; }
    public string? Genero { get; set; }
    public List<string> Generos { get; set; } = new();
    public string? Classificacao { get; set; }
    public double? AvaliacaoTmdb { get; set; }
    public string CapaUrl { get; set; } = "";
    public DateTime DataLancamento { get; set; }
    public int? DuracaoMinutos { get; set; }
    public string? TrailerUrl { get; set; }
    public string? Realizador { get; set; }
    public List<string> Atores { get; set; } = new();
    public List<TmdbReviewDTO> Reviews { get; set; } = new();
}

public class TmdbGeneroDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class TmdbReviewDTO
{
    public string Autor { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public string? Url { get; set; }
    public DateTime? CriadaEm { get; set; }
    public double? Nota { get; set; }
}
