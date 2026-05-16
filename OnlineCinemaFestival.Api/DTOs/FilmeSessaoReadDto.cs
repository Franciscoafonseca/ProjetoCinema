namespace OnlineCinemaFestival.Api.DTOs;

public class FilmeSessaoReadDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime? HoraInicio { get; set; }
    public DateTime? HoraFim { get; set; }
    public int Ordem { get; set; }
}
