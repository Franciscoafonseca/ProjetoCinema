using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Client.Models;

public class ListaPessoalDto
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Tipo { get; set; }

    public string TipoNome { get; set; } = string.Empty;

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int TotalFilmes { get; set; }

    public List<ListaPessoalItemDto> Items { get; set; } = new();
}

public class ListaPessoalItemDto
{
    public int FilmeId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string? Genero { get; set; }

    public string CapaUrl { get; set; } = string.Empty;

    public DateTime AddedAt { get; set; }
}

public class CriarListaRequest
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int Tipo { get; set; } = 0;

    public bool IsPublic { get; set; } = false;
}

public static class TipoListaPessoal
{
    public const int Custom = 0;
    public const int Watchlist = 1;
    public const int Watched = 2;
    public const int Favorites = 3;
}
