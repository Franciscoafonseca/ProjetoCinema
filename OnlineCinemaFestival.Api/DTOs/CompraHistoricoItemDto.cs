using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class CompraHistoricoItemDto
{
    public TipoAcesso Tipo { get; set; }
    public int? FilmeId { get; set; }
    public int? SessaoId { get; set; }
    public decimal PrecoPago { get; set; }
    public DateTime? Validade { get; set; }
}