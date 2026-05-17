namespace OnlineCinemaFestival.Client.Models;

public class FestivalDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public List<FestivalFilmeDTO> FilmesDoFestival { get; set; } = new();

    public List<ResultadoPremioFestivalDTO> ResultadosPremiosPublicados { get; set; } = new();
}

public class FestivalFilmeDTO
{
    public int FestivalId { get; set; }

    public int FilmeId { get; set; }

    public string TituloFilme { get; set; } = string.Empty;

    public bool ElegivelPremiosPublico { get; set; }

    public string? Secao { get; set; }

    public string? Categoria { get; set; }

    public DateTime DataAdicao { get; set; }

    public FilmeDTO? Filme { get; set; }
}
