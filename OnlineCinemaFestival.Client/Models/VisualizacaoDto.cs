namespace OnlineCinemaFestival.Client.Models;

public class VisualizacaoDto
{
    public string TipoConteudo { get; set; } = string.Empty;

    public int? FilmeId { get; set; }

    public int? SessaoId { get; set; }

    public bool TemChatAoVivo { get; set; }

    public string Mensagem { get; set; } = string.Empty;

    public List<ConteudoVisualizacaoDto> Conteudos { get; set; } = new();
}

public class ConteudoVisualizacaoDto
{
    public int FilmeId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string PosterUrl { get; set; } = string.Empty;

    public int Ordem { get; set; }

    public string UrlVisualizacao { get; set; } = string.Empty;
}

public class VisualizacaoHistoricoDto
{
    public int Id { get; set; }

    public int FilmeId { get; set; }

    public string FilmeTitulo { get; set; } = string.Empty;

    public string FilmePosterUrl { get; set; } = string.Empty;

    public int? SessaoId { get; set; }

    public DateTime? SessaoInicio { get; set; }

    public DateTime? SessaoFim { get; set; }

    public int? FestivalId { get; set; }

    public string FestivalNome { get; set; } = string.Empty;

    public string TipoConteudo { get; set; } = string.Empty;

    public string TipoAcessoUsado { get; set; } = string.Empty;

    public string? UrlVisualizacao { get; set; }

    public DateTime VisualizadoEm { get; set; }
}

public class ValidacaoAcessoDto
{
    public bool TemAcesso { get; set; }

    public string Mensagem { get; set; } = string.Empty;
}
