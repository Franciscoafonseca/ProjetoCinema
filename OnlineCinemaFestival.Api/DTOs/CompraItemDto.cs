using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.DTOs;

public class CompraItemDto
{
    public TipoAcesso Tipo { get; set; }
    public int? FilmeId { get; set; }
    public int? SessaoId { get; set; }
}
