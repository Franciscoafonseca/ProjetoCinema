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

    public string NomeFestival { get; set; } = string.Empty;

    public int? FilmeId { get; set; }

    public string TituloFilme { get; set; } = string.Empty;

    public DateTime? InicioSessao { get; set; }

    public DateTime? FimSessao { get; set; }

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

    public int Quantidade { get; set; } = 1;
}

public class CarrinhoItemCreateDto
{
    public int TipoAcesso { get; set; }

    public int? FestivalId { get; set; }

    public int? FilmeId { get; set; }

    public int? SessaoId { get; set; }

    public DateTime? DataPasse { get; set; }

    public int Quantidade { get; set; } = 1;
}

public class CarrinhoItemUpdateDto
{
    public int Quantidade { get; set; } = 1;
}

public class CarrinhoResumoDto
{
    public int CarrinhoId { get; set; }

    public int NumeroItens { get; set; }

    public decimal Subtotal { get; set; }

    public List<ItemCarrinhoDto> Itens { get; set; } = new();
}

public class CarrinhoValidacaoDto
{
    public bool Valido { get; set; }

    public decimal Total { get; set; }

    public List<CarrinhoErroValidacaoDto> Erros { get; set; } = new();

    public List<string> Avisos { get; set; } = new();
}

public class CarrinhoErroValidacaoDto
{
    public int? ItemId { get; set; }

    public string Campo { get; set; } = string.Empty;

    public string Mensagem { get; set; } = string.Empty;
}

public static class TiposAcesso
{
    public const int BilheteSessao = 1;
    public const int PasseDiario = 2;
    public const int PasseCompleto = 3;
    public const int AluguerDigital = 4;
}
