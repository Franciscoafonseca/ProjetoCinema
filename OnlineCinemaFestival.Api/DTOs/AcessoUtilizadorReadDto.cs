using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class AcessoUtilizadorDTO
{
    public int Id { get; set; }

    public int AcessoId { get; set; }

    public string NomeAcesso { get; set; } = string.Empty;

    public TipoAcesso TipoAcesso { get; set; }

    public string TipoAcessoNome { get; set; } = string.Empty;

    public int? SessaoId { get; set; }

    public int? FestivalId { get; set; }

    public int? FilmeId { get; set; }

    public string TituloFilme { get; set; } = string.Empty;

    public string NomeFestival { get; set; } = string.Empty;

    public DateTime? InicioSessao { get; set; }

    public DateTime? FimSessao { get; set; }

    public DateTime? DataAcesso { get; set; }

    public int? DuracaoHoras { get; set; }

    public DateTime InicioValidade { get; set; }

    public DateTime FimValidade { get; set; }

    public bool Ativo { get; set; }
}

public class AcessoUtilizadorReadDTO : AcessoUtilizadorDTO { }
