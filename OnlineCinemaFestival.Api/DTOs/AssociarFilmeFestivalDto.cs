using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class AssociarFilmeFestivalDTO
{
    [Required]
    public int FilmeId { get; set; }

    public bool ElegivelPremiosPublico { get; set; }

    [MaxLength(150)]
    public string? Secao { get; set; }

    [MaxLength(150)]
    public string? Categoria { get; set; }
}

public class FestivalFilmeReadDTO
{
    public int FestivalId { get; set; }

    public int FilmeId { get; set; }

    public string TituloFilme { get; set; } = string.Empty;

    public bool ElegivelPremiosPublico { get; set; }

    public string? Secao { get; set; }

    public string? Categoria { get; set; }

    public DateTime DataAdicao { get; set; }

    public FilmeResumoDTO? Filme { get; set; }
}
