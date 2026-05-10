using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class SessaoReadDto
{
    public int Id { get; set; }

    public int FestivalId { get; set; }

    public string FestivalName { get; set; } = string.Empty;

    public int FilmeId { get; set; }

    public string FilmeTitulo { get; set; } = string.Empty;

    public TipoSessao Tipo { get; set; }

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }

    public string? Observacoes { get; set; }
}
