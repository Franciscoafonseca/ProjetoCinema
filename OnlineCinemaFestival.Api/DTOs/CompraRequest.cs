namespace OnlineCinemaFestival.Api.DTOs;

public class CompraRequest
{
    public int UtilizadorId { get; set; }
    public List<CompraItemDto> Itens { get; set; } = new();
}
