using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class AdicionarItemCarrinhoDto
{
    [Required]
    public int AcessoId { get; set; }

    [Range(1, 99, ErrorMessage = "A quantidade deve estar entre 1 e 99.")]
    public int Quantidade { get; set; } = 1;
}
