using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class AcessoReadDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public TipoAcesso Tipo { get; set; }

    public string TipoNome { get; set; } = string.Empty;

    public decimal Preco { get; set; }

    public bool IsAtivo { get; set; }

    public int? SessaoId { get; set; }

    public int? FestivalId { get; set; }

    public string FestivalNome { get; set; } = string.Empty;

    public int? FilmeId { get; set; }

    public string FilmeTitulo { get; set; } = string.Empty;

    public DateTime? DataAcesso { get; set; }

    public int? DuracaoHoras { get; set; }

    public DateTime CriadoEm { get; set; }
}
