namespace OnlineCinemaFestival.Api.DTOs;

public class CatalogoQueryDTO
{
    public int? FestivalId { get; set; }

    public string? Pesquisa { get; set; }

    public string? Genero { get; set; }

    public CatalogoOrdenacao OrdenarPor { get; set; } = CatalogoOrdenacao.Titulo;

    public bool Descendente { get; set; } = false;
}
