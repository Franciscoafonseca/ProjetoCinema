using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class ComentarioCreateDTO
{
    [Required(ErrorMessage = "O comentario e obrigatorio.")]
    [StringLength(
        600,
        MinimumLength = 3,
        ErrorMessage = "O comentario deve ter entre 3 e 600 caracteres."
    )]
    public string Texto { get; set; } = string.Empty;
}
