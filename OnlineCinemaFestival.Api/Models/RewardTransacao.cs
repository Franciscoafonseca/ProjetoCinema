using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class RewardTransacao
{
    public int Id { get; set; }

    [Required]
    public string UtilizadorId { get; set; } = string.Empty;

    public int Pontos { get; set; }

    public DateTime Data { get; set; } = DateTime.UtcNow;

    [MaxLength(200)]
    public string Motivo { get; set; } = string.Empty;
}
