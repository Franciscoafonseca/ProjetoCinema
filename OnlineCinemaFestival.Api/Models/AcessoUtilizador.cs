namespace OnlineCinemaFestival.Api.Models;

public class AcessoUtilizador
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; } = null!;

    public int AcessoId { get; set; }
    public Acesso Acesso { get; set; } = null!;

    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;

    public TipoAcesso TipoAcesso { get; set; }

    public int? SessaoId { get; set; }
    public Sessao? Sessao { get; set; }

    public int? FestivalId { get; set; }
    public Festival? Festival { get; set; }

    public int? FilmeId { get; set; }
    public Filme? Filme { get; set; }

    public DateTime InicioValidade { get; set; }

    public DateTime FimValidade { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
