using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class CompraDTO
{
    public int Id { get; set; }

    public string Referencia { get; set; } = string.Empty;

    public int UtilizadorId { get; set; }

    public decimal ValorTotal { get; set; }

    public EstadoCompra Estado { get; set; }

    public string EstadoNome { get; set; } = string.Empty;

    public DateTime CriadaEm { get; set; }

    public DateTime? PagaEm { get; set; }

    public PagamentoReadDTO? Pagamento { get; set; }

    public List<ItemCompraReadDTO> Itens { get; set; } = new();
}

public class CompraReadDTO : CompraDTO { }
