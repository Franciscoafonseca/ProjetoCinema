using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class ListaPessoalReadDto
{
    public int Id { get; set; }

    public int UtilizadorId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TipoListaPessoal Tipo { get; set; }

    public string TipoNome { get; set; } = string.Empty;

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int TotalFilmes { get; set; }

    public List<ListaPessoalItemReadDto> Items { get; set; } = new();
}
