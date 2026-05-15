using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class CompraReadDto
{
    public int Id { get; set; }

    public string Referencia { get; set; } = string.Empty;

    public int UtilizadorId { get; set; }

    public decimal ValorTotal { get; set; }

    public EstadoCompra Estado { get; set; }

    public string EstadoNome { get; set; } = string.Empty;

    public DateTime CriadaEm { get; set; }

    public DateTime? PagaEm { get; set; }

    public PagamentoReadDto? Pagamento { get; set; }

    public List<ItemCompraReadDto> Itens { get; set; } = new();
}
