namespace OnlineCinemaFestival.Api.Models;

public class RewardTransacao
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }

    public int Pontos { get; set; }

    public DateTime Data { get; set; } = DateTime.UtcNow;

    public string Motivo { get; set; } = string.Empty;
}
