namespace OnlineCinemaFestival.Api.DTOs;

public class FestivalResumoDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}

public class FestivalDetalheDTO : FestivalResumoDTO
{
    public string Description { get; set; } = string.Empty;

    public string? Premios { get; set; }

    public List<FilmeResumoDTO> Filmes { get; set; } = new();

    public List<FestivalFilmeReadDTO> FilmesDoFestival { get; set; } = new();

    public List<SessaoResumoDTO> Sessoes { get; set; } = new();

    public List<AcessoReadDTO> PassesDisponiveis { get; set; } = new();

    public List<ResultadoPremioFestivalDTO> ResultadosPremiosPublicados { get; set; } = new();
}

public class FestivalReadDTO : FestivalDetalheDTO { }
