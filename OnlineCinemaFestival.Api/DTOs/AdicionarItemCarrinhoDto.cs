using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class AdicionarItemCarrinhoDto
{
    [Required]
    public int AcessoId { get; set; }
}
