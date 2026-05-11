using System.ComponentModel.DataAnnotations;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class SessaoUpdateDto
{
    [Required]
    [MinLength(1, ErrorMessage = "A sessão deve ter pelo menos um filme.")]
    public List<int> FilmeIds { get; set; } = new();

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
