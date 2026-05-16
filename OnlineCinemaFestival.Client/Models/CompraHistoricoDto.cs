namespace OnlineCinemaFestival.Client.Models;

public class CompraHistoricoDto
{
    public int Id { get; set; }
    public string UtilizadorId { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public decimal Total { get; set; }
    public int PontosGanhos { get; set; }
    public List<CompraHistoricoItemDto> Itens { get; set; } = new();
}

public class CompraHistoricoItemDto
{
    public int Id { get; set; }
    public int? FilmeId { get; set; }
    public int? SessaoId { get; set; }
    public int TipoAcesso { get; set; }
    public decimal PrecoPago { get; set; }
    public DateTime? Validade { get; set; }
}
