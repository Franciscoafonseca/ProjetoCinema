namespace OnlineCinemaFestival.Api.DTOs;

public class FilmeSessaoReadDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int Ordem { get; set; }
}
