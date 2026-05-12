using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class AcessoUtilizadorReadDto
{
    public int Id { get; set; }

    public int AcessoId { get; set; }

    public string NomeAcesso { get; set; } = string.Empty;

    public TipoAcesso TipoAcesso { get; set; }

    public string TipoAcessoNome { get; set; } = string.Empty;

    public int? SessaoId { get; set; }

    public int? FestivalId { get; set; }

    public int? FilmeId { get; set; }

    public DateTime InicioValidade { get; set; }

    public DateTime FimValidade { get; set; }

    public bool Ativo { get; set; }
}
