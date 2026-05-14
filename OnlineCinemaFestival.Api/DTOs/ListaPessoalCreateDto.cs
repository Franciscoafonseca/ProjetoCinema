using System.ComponentModel.DataAnnotations;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class ListaPessoalCreateDto
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public TipoListaPessoal Tipo { get; set; } = TipoListaPessoal.Custom;

    public bool IsPublic { get; set; } = false;
}
