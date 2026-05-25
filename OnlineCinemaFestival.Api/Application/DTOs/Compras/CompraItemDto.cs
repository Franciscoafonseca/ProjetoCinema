using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Application.DTOs;

public class CompraItemDto
{
    public TipoAcesso Tipo { get; set; }
    public int? FilmeId { get; set; }
    public int? SessaoId { get; set; }
}
