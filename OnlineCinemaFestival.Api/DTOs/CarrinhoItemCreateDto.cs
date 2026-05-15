using System.ComponentModel.DataAnnotations;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class CarrinhoItemCreateDto
{
    [Required]
    public TipoAcesso TipoAcesso { get; set; }

    public int? FestivalId { get; set; }

    public int? FilmeId { get; set; }

    public int? SessaoId { get; set; }

    public DateTime? DataPasse { get; set; }

    [Range(1, 99, ErrorMessage = "A quantidade deve estar entre 1 e 99.")]
    public int Quantidade { get; set; } = 1;
}
