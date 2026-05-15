namespace OnlineCinemaFestival.Api.DTOs;

public class SessaoEstadoReadDto
{
    public int SessaoId { get; set; }

    public string Estado { get; set; } = string.Empty;

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }
}
