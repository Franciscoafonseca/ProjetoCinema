namespace OnlineCinemaFestival.Api.DTOs;

public class CarrinhoReadDto
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }

    public List<ItemCarrinhoReadDto> Itens { get; set; } = new();

    public decimal Total { get; set; }
}
