using System.ComponentModel.DataAnnotations;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class SessaoCreateDTO
{
    [Required]
    public int FestivalId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Seleciona um filme valido.")]
    public int FilmeId { get; set; }

    [Required]
    public TipoSessao Tipo { get; set; }

    [Required]
    public DateTime Inicio { get; set; }

    [Required]
    public DateTime Fim { get; set; }

    public bool TemChatAoVivo { get; set; } = true;

    [MaxLength(500)]
    public string? Observacoes { get; set; }
}
