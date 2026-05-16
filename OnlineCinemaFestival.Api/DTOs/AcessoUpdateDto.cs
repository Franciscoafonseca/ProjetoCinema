using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class AcessoUpdateDTO
{
    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descricao { get; set; }

    [Range(0, 9999)]
    public decimal Preco { get; set; }

    public bool IsAtivo { get; set; }
}
