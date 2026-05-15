using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class RegistarVisualizacaoDto
{
    [Required]
    public int FilmeId { get; set; }

    public int? SessaoId { get; set; }

    public int? FestivalId { get; set; }

    public string? UrlVisualizacao { get; set; }
}
