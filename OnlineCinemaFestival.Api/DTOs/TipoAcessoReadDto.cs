using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class TipoAcessoReadDTO
{
    public TipoAcesso Tipo { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}
