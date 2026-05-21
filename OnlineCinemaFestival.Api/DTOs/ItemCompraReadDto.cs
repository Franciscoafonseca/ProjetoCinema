using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class ItemCompraReadDTO
{
    public int Id { get; set; }

    public int AcessoId { get; set; }

    public string NomeAcesso { get; set; } = string.Empty;

    public TipoAcesso TipoAcesso { get; set; }

    public string TipoAcessoNome { get; set; } = string.Empty;

    public decimal PrecoUnitario { get; set; }

    public int Quantidade { get; set; }

    public decimal Subtotal { get; set; }

    public int? SessaoId { get; set; }

    public DateTime? InicioSessao { get; set; }

    public DateTime? FimSessao { get; set; }

    public int? FestivalId { get; set; }

    public string NomeFestival { get; set; } = string.Empty;

    public int? FilmeId { get; set; }

    public string TituloFilme { get; set; } = string.Empty;

    public DateTime? DataAcesso { get; set; }

    public DateTime? InicioValidade { get; set; }

    public DateTime? FimValidade { get; set; }
}
