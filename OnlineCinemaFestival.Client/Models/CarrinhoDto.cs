namespace OnlineCinemaFestival.Client.Models;

public class CarrinhoDto
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }

    public List<ItemCarrinhoDto> Itens { get; set; } = new();

    public decimal Total { get; set; }
}

public class ItemCarrinhoDto
{
    public int Id { get; set; }

    public int AcessoId { get; set; }

    public string NomeAcesso { get; set; } = string.Empty;

    public int TipoAcesso { get; set; }

    public string TipoAcessoNome { get; set; } = string.Empty;

    public int? SessaoId { get; set; }

    public int? FestivalId { get; set; }

    public int? FilmeId { get; set; }

    public DateTime? DataAcesso { get; set; }

    public int? DuracaoHoras { get; set; }

    public decimal PrecoUnitario { get; set; }

    public int Quantidade { get; set; }

    public decimal Subtotal { get; set; }

    public DateTime DataAdicao { get; set; }
}

public class AdicionarItemCarrinhoDto
{
    public int AcessoId { get; set; }
}
