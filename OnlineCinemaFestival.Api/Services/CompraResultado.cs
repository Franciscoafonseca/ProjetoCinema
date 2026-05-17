using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public class CompraResultado
{
    public decimal Total { get; init; }
    public int PontosGanhos { get; init; }
    public List<CompraHistoricoItemDto> Itens { get; init; } = new();
}