using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class SessaoReadDto
{
    public int Id { get; set; }

    public int FestivalId { get; set; }
    public string FestivalName { get; set; } = string.Empty;

    public TipoSessao Tipo { get; set; }

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }

    public bool TemChatAoVivo { get; set; }

    public string? Observacoes { get; set; }

    public List<FilmeSessaoReadDto> Filmes { get; set; } = new();
}
