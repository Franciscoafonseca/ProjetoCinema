namespace OnlineCinemaFestival.Api.DTOs
{
    public class FilmeResumoDto
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Titulo { get; set; } = "";
        public string? Sinopse { get; set; }
        public DateTime DataLancamento { get; set; }
        public string? Genero { get; set; }
        public string? Classificacao { get; set; }
        public string CapaUrl { get; set; } = "";
        public string? TrailerUrl { get; set; }
        public string? VideoUrl { get; set; }
        public int Popularidade { get; set; }
    }

    public class FilmeDetalheDto : FilmeResumoDto { }

    public class FilmeReadDto : FilmeDetalheDto { }
}
