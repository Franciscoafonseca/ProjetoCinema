using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class PedidoRegistoDTO
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [MaxLength(2)]
    public string CountryCode { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Nationality { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string Location { get; set; } = string.Empty;
}
