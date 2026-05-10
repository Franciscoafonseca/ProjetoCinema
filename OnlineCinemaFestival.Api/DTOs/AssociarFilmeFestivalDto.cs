using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class AssociarFilmeFestivalDto
{
    [Required]
    public int FilmeId { get; set; }
}
