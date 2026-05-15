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
}

public class FestivalReadDto : FestivalDetalheDto { }
