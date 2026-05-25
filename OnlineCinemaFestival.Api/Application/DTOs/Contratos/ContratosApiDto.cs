using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Application.DTOs;

// Aliases mantidos para preservar os nomes dos contratos usados pelo cliente.
public class CriarFestivalDTO : FestivalCreateDTO { }

public class AtualizarFestivalDTO : FestivalUpdateDTO { }

public class CriarSessaoDTO : SessaoCreateDTO { }

public class CriarAcessoDTO : AcessoCreateDTO { }

public class CriarComentarioDTO : ComentarioCreateDTO { }

public class PedidoFinalizacaoCompraDTO { }

public class PedidoFinalizarCompraDTO
{
    [Required]
    public string MetodoPagamento { get; set; } = string.Empty;
}

public class ResultadoFinalizacaoCompraDTO : CompraReadDTO
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

public class AcessoAtivoDTO : AcessoUtilizadorReadDTO { }

public class CriarAvaliacaoDTO
{
    [Range(1, 10, ErrorMessage = "A pontuacao deve estar entre 1 e 10 estrelas.")]
    public int Pontuacao { get; set; }

    [Required(ErrorMessage = "O texto da review e obrigatorio.")]
    [StringLength(
        1000,
        MinimumLength = 20,
        ErrorMessage = "A review deve ter entre 20 e 1000 caracteres."
    )]
    public string Texto { get; set; } = string.Empty;
}

public class AvaliacaoDTO
{
    public int Id { get; set; }

    public int FilmeId { get; set; }

    public string TituloFilme { get; set; } = string.Empty;

    public int UsuarioId { get; set; }

    public string NomeUsuario { get; set; } = string.Empty;

    public int Pontuacao { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTime Data { get; set; }
}

public class ReportarComentarioDTO
{
    [MaxLength(500)]
    public string Motivo { get; set; } = string.Empty;
}

public class GeneroDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class CriarGeneroDTO
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
