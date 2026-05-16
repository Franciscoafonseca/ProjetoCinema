using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class AssociarFilmeFestivalDTO
{
    [Required]
    public int FilmeId { get; set; }
}
