namespace OnlineCinemaFestival.Api.DTOs;

public class ListaPessoalItemReadDTO
{
    public int FilmeId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string? Genero { get; set; }

    public string CapaUrl { get; set; } = string.Empty;

    public DateTime AddedAt { get; set; }
}
