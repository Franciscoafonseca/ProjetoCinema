namespace OnlineCinemaFestival.Api.DTOs;

public class CarrinhoDTO
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }

    public List<ItemCarrinhoReadDTO> Itens { get; set; } = new();

    public decimal Total { get; set; }
}

public class CarrinhoReadDTO : CarrinhoDTO { }
