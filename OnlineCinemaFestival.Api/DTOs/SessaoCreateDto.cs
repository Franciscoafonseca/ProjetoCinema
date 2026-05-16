using System.ComponentModel.DataAnnotations;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class SessaoCreateDTO
{
    [Required]
    public int FestivalId { get; set; }

    [MinLength(1, ErrorMessage = "A sessao deve ter pelo menos um filme.")]
    public List<int> FilmeIds { get; set; } = new();

    public List<SessaoFilmeCreateDTO> Filmes { get; set; } = new();

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

public class SessaoFilmeCreateDTO
{
    [Required]
    public int FilmeId { get; set; }

    public DateTime? HoraInicio { get; set; }

    public DateTime? HoraFim { get; set; }

    public int? Ordem { get; set; }
}
