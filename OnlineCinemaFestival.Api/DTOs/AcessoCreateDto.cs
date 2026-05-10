using System.ComponentModel.DataAnnotations;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class AcessoCreateDto
{
    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descricao { get; set; }

    [Required]
    public TipoAcesso Tipo { get; set; }

    [Range(0, 9999)]
    public decimal Preco { get; set; }

    public int? SessaoId { get; set; }

    public int? FestivalId { get; set; }

    public int? FilmeId { get; set; }

    public DateTime? DataAcesso { get; set; }

    public int? DuracaoHoras { get; set; }
}
