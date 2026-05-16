using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class PagamentoReadDTO
{
    public int Id { get; set; }

    public string Referencia { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public string Metodo { get; set; } = string.Empty;

    public EstadoPagamento Estado { get; set; }

    public string EstadoNome { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; }

    public DateTime? ProcessadoEm { get; set; }

    public string? Mensagem { get; set; }
}
