namespace OnlineCinemaFestival.Api.DTOs
{
    public class FilmeResumoDto
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

    public class FilmeDetalheDto : FilmeResumoDto
    {
        public string? Realizador { get; set; }
        public List<string> Atores { get; set; } = new();
        public List<TmdbReviewDto> ReviewsTmdb { get; set; } = new();
        public List<AvaliacaoDto> ReviewsAplicacao { get; set; } = new();
        public List<FestivalResumoDto> Festivais { get; set; } = new();
        public string? Premios { get; set; }
        public List<FilmeSessaoReadDto> Sessoes { get; set; } = new();
        public List<AcessoReadDto> AcessosDisponiveis { get; set; } = new();
        public bool PodeVer { get; set; }
        public bool PodeAvaliar { get; set; }
    }

    public class FilmeReadDto : FilmeDetalheDto { }
}
