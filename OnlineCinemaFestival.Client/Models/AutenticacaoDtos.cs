using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Client.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Palavra-passe é obrigatória.")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    [MaxLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Palavra-passe é obrigatória.")]
    [MinLength(6, ErrorMessage = "A palavra-passe tem de ter pelo menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Nationality { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;

    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
