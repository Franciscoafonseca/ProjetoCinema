using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Application.DTOs;

public class PedidoLoginDTO
{
    [Required(ErrorMessage = "O email e obrigatorio.")]
    [EmailAddress(ErrorMessage = "Introduz um email valido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A palavra-passe e obrigatoria.")]
    public string Password { get; set; } = string.Empty;
}
