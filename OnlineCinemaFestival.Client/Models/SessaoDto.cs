namespace OnlineCinemaFestival.Client.Models;

public class SessaoDto
{
    public int Id { get; set; }

    public int FestivalId { get; set; }

    public string FestivalName { get; set; } = string.Empty;

    public string NomeFestival { get; set; } = string.Empty;

    public int? FilmeId { get; set; }

    public string TituloFilme { get; set; } = string.Empty;

    public string FilmeTitulo { get; set; } = string.Empty;

    public int Tipo { get; set; }

    public string TipoNome { get; set; } = string.Empty;

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }

    public string Estado { get; set; } = string.Empty;

    public bool TemChatAoVivo { get; set; }

    public string? Observacoes { get; set; }

    public List<FilmeSessaoDto> Filmes { get; set; } = new();

    public string Festival =>
        !string.IsNullOrWhiteSpace(NomeFestival) ? NomeFestival : FestivalName;

    public string Filme =>
        !string.IsNullOrWhiteSpace(TituloFilme) ? TituloFilme
        : !string.IsNullOrWhiteSpace(FilmeTitulo) ? FilmeTitulo
        : Filmes.FirstOrDefault()?.Titulo ?? string.Empty;

    public int? FilmePrincipalId => FilmeId ?? Filmes.FirstOrDefault()?.Id;
}

public class FilmeSessaoDto
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public int Ordem { get; set; }
}

public class SessaoEstadoDto
{
    public int SessaoId { get; set; }

    public string Estado { get; set; } = string.Empty;

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }
}
