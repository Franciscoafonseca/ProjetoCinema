namespace OnlineCinemaFestival.Client.Models;

public class RewardTransacaoDto
{
    public int Id { get; set; }
    public int UtilizadorId { get; set; }
    public int Pontos { get; set; }
    public DateTime Data { get; set; }
    public string Motivo { get; set; } = string.Empty;
}
