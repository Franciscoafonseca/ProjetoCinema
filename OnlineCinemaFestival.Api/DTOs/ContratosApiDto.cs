using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.DTOs;

public class CriarFestivalDto : FestivalCreateDto { }

public class AtualizarFestivalDto : FestivalUpdateDto { }

public class CriarSessaoDto : SessaoCreateDto { }

public class CriarAcessoDto : AcessoCreateDto { }

public class CriarComentarioDto : ComentarioCreateDto { }

public class CheckoutRequestDto { }

public class FinalizarCheckoutRequest
{
    [Required]
    public string MetodoPagamento { get; set; } = "CartaoCredito";
}

public class CheckoutResultadoDto : CompraReadDto
{
    public string Mensagem { get; set; } = string.Empty;

    public int AcessosGerados { get; set; }
}

public class CompraResumoDto
{
    public int Id { get; set; }

    public string Referencia { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public string EstadoNome { get; set; } = string.Empty;

    public DateTime CriadaEm { get; set; }

    public int NumeroItens { get; set; }
}

public class AcessoAtivoDto : AcessoUtilizadorReadDto { }

public class CriarAvaliacaoDto
{
    [Range(1, 10)]
    public int Pontuacao { get; set; }

    [MaxLength(2000)]
    public string Texto { get; set; } = string.Empty;
}

public class AvaliacaoDto
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

public class ReportarComentarioDto
{
    [MaxLength(500)]
    public string Motivo { get; set; } = string.Empty;
}

public class GeneroDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class CriarGeneroDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
