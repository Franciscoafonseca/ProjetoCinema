using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class Reward
{
    public int Id { get; set; }

    [Required]
    public string UtilizadorId { get; set; } = string.Empty;

    public int Pontos { get; set; }
}
