using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class CarrinhoItemUpdateDto
{
    [Range(1, 99, ErrorMessage = "A quantidade deve estar entre 1 e 99.")]
    public int Quantidade { get; set; } = 1;
}
