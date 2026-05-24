using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Application.DTOs;

public class TipoAcessoReadDTO
{
    public TipoAcesso Tipo { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}
