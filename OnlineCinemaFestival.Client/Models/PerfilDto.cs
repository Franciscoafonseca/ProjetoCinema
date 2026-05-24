using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Client.Models;

public class PerfilPublicoDTO
{
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Nationality { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string CountryFlag { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public bool IsPublic { get; set; }

    public List<string> FavoriteGenres { get; set; } = new();

    public int ReviewsCount { get; set; }

    public int CommunitiesCount { get; set; }

    public int PublicListsCount { get; set; }

    public List<ListaPessoalPublicaDTO> PublicLists { get; set; } = new();

    public List<ReviewPublicaDTO> PublicReviews { get; set; } = new();

    public List<ComunidadePublicaPerfilDTO> PublicCommunities { get; set; } = new();
}

public class PerfilPrivadoDTO : PerfilPublicoDTO
{
    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}

public class PerfilUtilizadorRespostaDTO : PerfilPrivadoDTO { }

public class ListaPessoalPublicaDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int TotalFilmes { get; set; }
}

public class ReviewPublicaDTO
{
    public int Id { get; set; }

    public int FilmeId { get; set; }

    public string FilmeTitulo { get; set; } = string.Empty;

    public int Pontuacao { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTime Data { get; set; }
}

public class ComunidadePublicaPerfilDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int MembersCount { get; set; }
}

public class CriarReporteUtilizadorDTO
{
    [Required(ErrorMessage = "Indica o motivo do reporte.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "O motivo deve ter entre 10 e 500 caracteres.")]
    public string Motivo { get; set; } = string.Empty;
}

public class ReporteUtilizadorDTO
{
    public int Id { get; set; }

    public EstadoReporteUtilizador Estado { get; set; }

    public string Motivo { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; }

    public DateTime? AtualizadoEm { get; set; }

    public int UtilizadorReportadoId { get; set; }

    public string UtilizadorReportadoNome { get; set; } = string.Empty;

    public int ReportadoPorUtilizadorId { get; set; }

    public string ReportadoPorUtilizadorNome { get; set; } = string.Empty;
}

public class AtualizarEstadoReporteUtilizadorDTO
{
    public EstadoReporteUtilizador Estado { get; set; }
}

public enum EstadoReporteUtilizador
{
    Pendente = 0,
    Analisado = 1,
    Rejeitado = 2,
    AcaoAplicada = 3,
}

public class PedidoAtualizarPerfilDTO
{
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [RegularExpression(@"^\+?[0-9\s().-]{7,30}$", ErrorMessage = "Introduz um telefone valido.")]
    [MaxLength(30, ErrorMessage = "O telefone nao pode exceder 30 caracteres.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(2)]
    public string CountryCode { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Nationality { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Bio { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Location { get; set; } = string.Empty;

    public bool IsPublic { get; set; } = true;

    public List<int> FavoriteGenreIds { get; set; } = new();
}

public class PaisOpcaoDTO
{
    public string Codigo { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;
}

public class PerfilOpcoesDTO
{
    public List<PaisOpcaoDTO> Paises { get; set; } = new();

    public List<string> Localidades { get; set; } = new();
}
