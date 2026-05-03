namespace OnlineCinemaFestival.Api.DTOs;

public class CompraRequest
{
    public string UtilizadorId { get; set; } = string.Empty;
    public List<CompraItemDto> Itens { get; set; } = new();
}
