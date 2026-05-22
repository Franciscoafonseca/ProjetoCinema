using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OnlineCinemaFestival.Client.Models;

public class PedidoLoginDTO
{
    [Required(ErrorMessage = "Email e obrigatorio.")]
    [EmailAddress(ErrorMessage = "Email invalido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Palavra-passe e obrigatoria.")]
    public string Password { get; set; } = string.Empty;
}

public class PedidoRegistoDTO
{
    [Required(ErrorMessage = "Nome e obrigatorio.")]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email e obrigatorio.")]
    [EmailAddress(ErrorMessage = "Email invalido.")]
    [MaxLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefone e obrigatorio.")]
    [RegularExpression(@"^\+?[0-9\s().-]{7,30}$", ErrorMessage = "Introduz um telefone valido.")]
    [MaxLength(30, ErrorMessage = "O telefone nao pode exceder 30 caracteres.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Palavra-passe e obrigatoria.")]
    [MinLength(8, ErrorMessage = "A palavra-passe tem de ter pelo menos 8 caracteres.")]
    [StrongPassword]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma a palavra-passe.")]
    [Compare(nameof(Password), ErrorMessage = "A confirmacao nao coincide.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pais/nacionalidade e obrigatorio.")]
    [MaxLength(2)]
    public string CountryCode { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Nationality { get; set; } = string.Empty;

    [Required(ErrorMessage = "Localidade e obrigatoria.")]
    [MaxLength(120)]
    public string Location { get; set; } = string.Empty;
}

public class AutenticacaoRespostaDTO
{
    public string Token { get; set; } = string.Empty;

    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}

public class PedidoAutenticacaoExternaDTO
{
    public string Provider { get; set; } = string.Empty;

    public string IdToken { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;
}

public class ProvedorAutenticacaoExternaDTO
{
    public string Provider { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public bool Configurado { get; set; }
}

public sealed class StrongPasswordAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var password = value as string ?? string.Empty;

        if (password.Length < 8)
            return new ValidationResult("A palavra-passe tem de ter pelo menos 8 caracteres.");

        if (!Regex.IsMatch(password, "[A-Z]")
            || !Regex.IsMatch(password, "[a-z]")
            || !Regex.IsMatch(password, "[0-9]")
            || !Regex.IsMatch(password, "[^a-zA-Z0-9]"))
        {
            return new ValidationResult("Usa maiusculas, minusculas, numeros e simbolos.");
        }

        return ValidationResult.Success;
    }
}
