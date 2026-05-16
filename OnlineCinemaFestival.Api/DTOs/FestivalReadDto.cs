namespace OnlineCinemaFestival.Api.DTOs;

public class FestivalResumoDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}

public class FestivalDetalheDto : FestivalResumoDto
{
    public string Description { get; set; } = string.Empty;

    public string? Premios { get; set; }

    public List<FilmeResumoDto> Filmes { get; set; } = new();

    public List<SessaoResumoDto> Sessoes { get; set; } = new();

    public List<AcessoReadDto> PassesDisponiveis { get; set; } = new();
}

public class FestivalReadDto : FestivalDetalheDto { }
