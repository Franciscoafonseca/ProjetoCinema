using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class SessaoResumoDto
{
    public int Id { get; set; }

    public int FestivalId { get; set; }
    public string FestivalName { get; set; } = string.Empty;
    public string NomeFestival { get; set; } = string.Empty;

    public int? FilmeId { get; set; }
    public string TituloFilme { get; set; } = string.Empty;

    public TipoSessao Tipo { get; set; }
    public string TipoNome { get; set; } = string.Empty;

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }

    public string Estado { get; set; } = string.Empty;

    public bool TemChatAoVivo { get; set; }

    public string? Observacoes { get; set; }

    public List<FilmeSessaoReadDto> Filmes { get; set; } = new();
}

public class SessaoDetalheDto : SessaoResumoDto { }

public class SessaoReadDto : SessaoDetalheDto { }
