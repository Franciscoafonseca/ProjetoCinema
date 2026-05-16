using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class ComentarioCreateDTO
{
    [Required]
    [StringLength(600, MinimumLength = 1)]
    public string Texto { get; set; } = string.Empty;
}
