namespace OnlineCinemaFestival.Client.Models;

public class FestivalDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string EstadoFestival { get; set; } = string.Empty;

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

public class CriarFestivalDTO
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(3);
}

public class AssociarFilmeFestivalDTO
{
    public int FilmeId { get; set; }

    public bool ElegivelPremiosPublico { get; set; } = true;

    public string? Secao { get; set; }

    public string? Categoria { get; set; }
}
