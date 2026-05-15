using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class Acesso
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descricao { get; set; }

    public TipoAcesso Tipo { get; set; }

    public decimal Preco { get; set; }

    public bool IsAtivo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Bilhete de sessão
    public int? SessaoId { get; set; }
    public Sessao? Sessao { get; set; }

    // Passe diário / passe completo
    public int? FestivalId { get; set; }
    public Festival? Festival { get; set; }

    // Aluguer digital
    public int? FilmeId { get; set; }
    public Filme? Filme { get; set; }

    // Passe diário
    public DateTime? DataAcesso { get; set; }

    // Aluguer digital
    public int? DuracaoHoras { get; set; }

    public ICollection<CarrinhoItem> CarrinhoItems { get; set; } = new List<CarrinhoItem>();

    public ICollection<ItemCompra> ItensCompra { get; set; } = new List<ItemCompra>();

    public ICollection<AcessoUtilizador> AcessosUtilizador { get; set; } =
        new List<AcessoUtilizador>();
}
