using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Application.DTOs;

public class PedidoAtualizarPerfilDTO
{
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [RegularExpression(@"^\+?[0-9\s().-]{7,30}$", ErrorMessage = "Introduz um telefone valido.")]
    [MaxLength(30, ErrorMessage = "O telefone nao pode exceder 30 caracteres.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(2)]
    public string CountryCode { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Nationality { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Bio { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Location { get; set; } = string.Empty;

    public bool IsPublic { get; set; } = true;

    public List<int> FavoriteGenreIds { get; set; } = new();
}
