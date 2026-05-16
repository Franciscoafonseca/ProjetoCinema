namespace OnlineCinemaFestival.Api.DTOs
{
    public class FilmeResumoDTO
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Titulo { get; set; } = "";
        public string? TituloOriginal { get; set; }
        public string? Sinopse { get; set; }
        public DateTime DataLancamento { get; set; }
        public int? DuracaoMinutos { get; set; }
        public string? Genero { get; set; }
        public List<string> Generos { get; set; } = new();
        public string? Classificacao { get; set; }
        public double? AvaliacaoTmdb { get; set; }
        public string CapaUrl { get; set; } = "";
        public string? TrailerUrl { get; set; }
        public string? VideoUrl { get; set; }
        public int Popularidade { get; set; }
    }

    public class FilmeDetalheDTO : FilmeResumoDTO
    {
        public string? Realizador { get; set; }
        public List<string> Atores { get; set; } = new();
        public List<TmdbReviewDTO> ReviewsTmdb { get; set; } = new();
        public List<AvaliacaoDTO> ReviewsAplicacao { get; set; } = new();
        public List<FestivalResumoDTO> Festivais { get; set; } = new();
        public string? Premios { get; set; }
        public List<FilmeSessaoReadDTO> Sessoes { get; set; } = new();
        public List<AcessoReadDTO> AcessosDisponiveis { get; set; } = new();
        public bool PodeVer { get; set; }
        public bool PodeAvaliar { get; set; }
    }

    public class FilmeReadDTO : FilmeDetalheDTO { }
}
