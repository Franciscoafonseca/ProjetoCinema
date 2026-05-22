using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class PedidoRegistoDTO
{
    [Required(ErrorMessage = "O nome e obrigatorio.")]
    [MaxLength(120, ErrorMessage = "O nome nao pode exceder 120 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email e obrigatorio.")]
    [EmailAddress(ErrorMessage = "Introduz um email valido.")]
    [MaxLength(180, ErrorMessage = "O email nao pode exceder 180 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone e obrigatorio.")]
    [RegularExpression(@"^\+?[0-9\s().-]{7,30}$", ErrorMessage = "Introduz um telefone valido.")]
    [MaxLength(30, ErrorMessage = "O telefone nao pode exceder 30 caracteres.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "A palavra-passe e obrigatoria.")]
    [MinLength(8, ErrorMessage = "A palavra-passe deve ter pelo menos 8 caracteres.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
        ErrorMessage = "A palavra-passe deve incluir maiusculas, minusculas, numeros e simbolos."
    )]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma a palavra-passe.")]
    [Compare(nameof(Password), ErrorMessage = "A confirmacao da palavra-passe nao coincide.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleciona o pais.")]
    [MaxLength(2, ErrorMessage = "Seleciona um pais valido.")]
    public string CountryCode { get; set; } = string.Empty;

    [MaxLength(80, ErrorMessage = "A nacionalidade nao pode exceder 80 caracteres.")]
    public string Nationality { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleciona a localidade.")]
    [MaxLength(120, ErrorMessage = "A localidade nao pode exceder 120 caracteres.")]
    public string Location { get; set; } = string.Empty;
}
