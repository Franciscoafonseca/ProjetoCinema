namespace OnlineCinemaFestival.Api.DTOs;

public class ComentarioCreateDto
{
    public int UsuarioId { get; set; }

    public int ComunidadeId { get; set; }

    public string Texto { get; set; } = string.Empty;
}
