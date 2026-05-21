namespace OnlineCinemaFestival.Client.Models;

public class CompraDTO
{
    public int Id { get; set; }

    public string Referencia { get; set; } = string.Empty;

    public int UtilizadorId { get; set; }

    public decimal ValorTotal { get; set; }

    public int Estado { get; set; }

    public string EstadoNome { get; set; } = string.Empty;

    public DateTime CriadaEm { get; set; }

    public DateTime? PagaEm { get; set; }

    public PagamentoDTO? Pagamento { get; set; }

    public List<ItemCompraDTO> Itens { get; set; } = new();
}

public class ResultadoFinalizacaoCompraDTO : CompraDTO
{
    public string Mensagem { get; set; } = string.Empty;

    public int AcessosGerados { get; set; }
}

public class CompraResumoDTO
{
    public int Id { get; set; }

    public string Referencia { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public string EstadoNome { get; set; } = string.Empty;

    public DateTime CriadaEm { get; set; }

    public int NumeroItens { get; set; }
}

public class ItemCompraDTO
{
    public int Id { get; set; }

    public int AcessoId { get; set; }

    public string NomeAcesso { get; set; } = string.Empty;

    public int TipoAcesso { get; set; }

    public string TipoAcessoNome { get; set; } = string.Empty;

    public decimal PrecoUnitario { get; set; }

    public int Quantidade { get; set; }

    public decimal Subtotal { get; set; }

    public int? SessaoId { get; set; }

    public DateTime? InicioSessao { get; set; }

    public DateTime? FimSessao { get; set; }

    public int? FestivalId { get; set; }

    public string NomeFestival { get; set; } = string.Empty;

    public int? FilmeId { get; set; }

    public string TituloFilme { get; set; } = string.Empty;

    public DateTime? DataAcesso { get; set; }

    public DateTime? InicioValidade { get; set; }

    public DateTime? FimValidade { get; set; }
}

public class PagamentoDTO
{
    public int Id { get; set; }

    public string Referencia { get; set; } = string.Empty;

    public string? Entidade { get; set; }

    public decimal Valor { get; set; }

    public string Metodo { get; set; } = string.Empty;

    public int Estado { get; set; }

    public string EstadoNome { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; }

    public DateTime? ProcessadoEm { get; set; }

    public string? Mensagem { get; set; }
}
