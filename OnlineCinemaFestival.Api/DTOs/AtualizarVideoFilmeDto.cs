namespace OnlineCinemaFestival.Api.DTOs;

public class AtualizarVideoFilmeDTO
{
    public string? VideoProvider { get; set; }

    public string? VideoKey { get; set; }

    public string? VideoUrl { get; set; }

    public int? DuracaoVideoSegundos { get; set; }
}
