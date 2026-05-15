namespace OnlineCinemaFestival.Api.Models;

public class Pagamento
{
    public int Id { get; set; }

    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;

    public string Referencia { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public string Metodo { get; set; } = "Simulado";

    public EstadoPagamento Estado { get; set; } = EstadoPagamento.Pendente;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessadoEm { get; set; }

    public string? Mensagem { get; set; }
}
