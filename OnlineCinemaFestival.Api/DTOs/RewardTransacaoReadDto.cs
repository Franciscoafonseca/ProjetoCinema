namespace OnlineCinemaFestival.Api.DTOs;

public class RewardTransacaoReadDto
{
    public int Id { get; set; }
    public string UtilizadorId { get; set; } = string.Empty;
    public int Pontos { get; set; }
    public DateTime Data { get; set; }
    public string Motivo { get; set; } = string.Empty;
}
