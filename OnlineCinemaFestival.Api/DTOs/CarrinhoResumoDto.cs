namespace OnlineCinemaFestival.Api.DTOs;

public class CarrinhoResumoDto
{
    public int CarrinhoId { get; set; }

    public int NumeroItens { get; set; }

    public decimal Subtotal { get; set; }

    public List<ItemCarrinhoReadDto> Itens { get; set; } = new();
}
