using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class CarrinhoItemDTO
{
    public int Id { get; set; }

    public int AcessoId { get; set; }

    public string NomeAcesso { get; set; } = string.Empty;

    public TipoAcesso TipoAcesso { get; set; }

    public string TipoAcessoNome { get; set; } = string.Empty;

    public int? SessaoId { get; set; }

    public int? FestivalId { get; set; }
    public string NomeFestival { get; set; } = string.Empty;

    public int? FilmeId { get; set; }
    public string TituloFilme { get; set; } = string.Empty;

    public DateTime? InicioSessao { get; set; }

    public DateTime? FimSessao { get; set; }

    public DateTime? DataAcesso { get; set; }

    public int? DuracaoHoras { get; set; }

    public decimal PrecoUnitario { get; set; }

    public int Quantidade { get; set; }

    public decimal Subtotal { get; set; }

    public DateTime DataAdicao { get; set; }
}

public class ItemCarrinhoReadDTO : CarrinhoItemDTO { }
