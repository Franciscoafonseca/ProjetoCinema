using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public enum TipoAcesso
{
    // Válido para uma sessão específica
    BilheteUnico = 1,

    // Acesso a todas as sessões de um dia específico
    PasseDiario = 2,

    // Acesso total a todo o catálogo do festival
    PasseCompleto = 3,

    // Janela temporal por filme (ex: 48h)
    AluguerDigital = 4
}

public class Acesso
{

    public int Id { get; set; }

    [Required]
    public string UtilizadorId { get; set; } = string.Empty;

    public int? FilmeId { get; set; }

    [Required]
    public TipoAcesso Tipo { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataExpiracao { get; set; }

    [Required]
    public decimal PrecoPago { get; set; }
}