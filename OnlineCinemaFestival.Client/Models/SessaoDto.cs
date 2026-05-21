namespace OnlineCinemaFestival.Client.Models;

public class SessaoDTO
{
    public int Id { get; set; }

    public int FestivalId { get; set; }

    public string FestivalName { get; set; } = string.Empty;

    public string NomeFestival { get; set; } = string.Empty;

    public int FilmeId { get; set; }

    public string TituloFilme { get; set; } = string.Empty;

    public string FilmeTitulo { get; set; } = string.Empty;

    public int Tipo { get; set; }

    public string TipoNome { get; set; } = string.Empty;

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }

    public string Estado { get; set; } = string.Empty;

    public bool TemChatAoVivo { get; set; }

    public decimal? PrecoBilhete { get; set; }

    public string? Observacoes { get; set; }

    public string Festival =>
        !string.IsNullOrWhiteSpace(NomeFestival) ? NomeFestival : FestivalName;

    public string Filme =>
        !string.IsNullOrWhiteSpace(TituloFilme) ? TituloFilme
        : !string.IsNullOrWhiteSpace(FilmeTitulo) ? FilmeTitulo
        : string.Empty;

    public int FilmePrincipalId => FilmeId;
}

public class SessaoEstadoDTO
{
    public int SessaoId { get; set; }

    public string Estado { get; set; } = string.Empty;

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }
}

public class CriarSessaoDTO
{
    public int FestivalId { get; set; }

    public int FilmeId { get; set; }

    public int Tipo { get; set; }

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }

    public bool TemChatAoVivo { get; set; } = true;

    public string? Observacoes { get; set; }
}
