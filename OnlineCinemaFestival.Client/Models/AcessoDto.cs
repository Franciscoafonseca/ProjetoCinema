namespace OnlineCinemaFestival.Client.Models;

public class AcessoDTO
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public int Tipo { get; set; }

    public string TipoNome { get; set; } = string.Empty;

    public decimal Preco { get; set; }

    public bool IsAtivo { get; set; }

    public int? SessaoId { get; set; }

    public int? FestivalId { get; set; }

    public string FestivalNome { get; set; } = string.Empty;

    public int? FilmeId { get; set; }

    public string FilmeTitulo { get; set; } = string.Empty;

    public DateTime? DataAcesso { get; set; }

    public int? DuracaoHoras { get; set; }

    public DateTime CriadoEm { get; set; }
}

public class TipoAcessoDTO
{
    public int Tipo { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}

public class AcessoUtilizadorDTO
{
    public int Id { get; set; }

    public int AcessoId { get; set; }

    public string NomeAcesso { get; set; } = string.Empty;

    public int TipoAcesso { get; set; }

    public string TipoAcessoNome { get; set; } = string.Empty;

    public int? SessaoId { get; set; }

    public int? FestivalId { get; set; }

    public int? FilmeId { get; set; }

    public DateTime InicioValidade { get; set; }

    public DateTime FimValidade { get; set; }

    public bool Ativo { get; set; }
}

public class AcessoAtivoDTO : AcessoUtilizadorDTO { }
