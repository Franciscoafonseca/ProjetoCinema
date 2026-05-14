using System.ComponentModel.DataAnnotations;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class SessaoUpdateDto
{
    [Required]
    public TipoSessao Tipo { get; set; }

    [Required]
    public DateTime Inicio { get; set; }

    [Required]
    public DateTime Fim { get; set; }

    [MaxLength(500)]
    public string? Observacoes { get; set; }
}
