namespace OnlineCinemaFestival.Api.DTOs;

public class CarrinhoResumoDTO
{
    public int CarrinhoId { get; set; }

    public int NumeroItens { get; set; }

    public decimal Subtotal { get; set; }

    public List<ItemCarrinhoReadDTO> Itens { get; set; } = new();
}
